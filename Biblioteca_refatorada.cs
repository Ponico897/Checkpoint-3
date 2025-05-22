public class Livro
{
    public string Titulo { get; set; }
    public string Autor { get; set; }
    public string ISBN { get; set; }
    public bool Disponivel { get; set; } = true;
}

public class Usuario
{
    public string Nome { get; set; }
    public int ID { get; set; }
}

public class Emprestimo
{
    public Livro Livro { get; set; }
    public Usuario Usuario { get; set; }
    public DateTime DataEmprestimo { get; set; }
    public DateTime DataDevolucaoPrevista { get; set; }
    public DateTime? DataDevolucaoEfetiva { get; set; }
}

public interface INotificador
{
    void Notificar(string destinatario, string assunto, string mensagem);
}

public interface ICalculadoraMulta
{
    double CalcularMulta(DateTime dataPrevista, DateTime dataEfetiva);
}

public class EmailNotificador : INotificador
{
    public void Notificar(string destinatario, string assunto, string mensagem)
    {
        Console.WriteLine($"Email enviado para {destinatario}. Assunto: {assunto}");
    }
}

public class SMSNotificador : INotificador
{
    public void Notificar(string destinatario, string assunto, string mensagem)
    {
        Console.WriteLine($"SMS enviado para {destinatario}: {mensagem}");
    }
}

public class CalculadoraMulta : ICalculadoraMulta
{
    public double CalcularMulta(DateTime dataPrevista, DateTime dataEfetiva)
    {
        if (dataEfetiva <= dataPrevista) return 0;
        return (dataEfetiva - dataPrevista).Days * 1.0;
    }
}

public class LivroService
{
    private List<Livro> livros = new List<Livro>();

    public void AdicionarLivro(string titulo, string autor, string isbn)
    {
        livros.Add(new Livro { Titulo = titulo, Autor = autor, ISBN = isbn });
    }

    public Livro BuscarPorISBN(string isbn) => livros.FirstOrDefault(l => l.ISBN == isbn);

    public List<Livro> ListarLivros() => livros;
}

public class UsuarioService
{
    private List<Usuario> usuarios = new List<Usuario>();
    private readonly IEnumerable<INotificador> notificadores;

    public UsuarioService(IEnumerable<INotificador> notificadores)
    {
        this.notificadores = notificadores;
    }

    public void AdicionarUsuario(string nome, int id)
    {
        usuarios.Add(new Usuario { Nome = nome, ID = id });
        Notificar(nome, "Bem-vindo à Biblioteca", "Você foi cadastrado com sucesso.");
    }

    public Usuario BuscarPorId(int id) => usuarios.FirstOrDefault(u => u.ID == id);

    private void Notificar(string destinatario, string assunto, string mensagem)
    {
        foreach (var notificador in notificadores)
            notificador.Notificar(destinatario, assunto, mensagem);
    }

    public List<Usuario> ListarUsuarios() => usuarios;
}

public class EmprestimoService
{
    private readonly List<Emprestimo> emprestimos = new List<Emprestimo>();
    private readonly IEnumerable<INotificador> notificadores;
    private readonly LivroService livroService;
    private readonly UsuarioService usuarioService;
    private readonly ICalculadoraMulta calculadoraMulta;

    public EmprestimoService(IEnumerable<INotificador> notificadores, LivroService livroService, UsuarioService usuarioService, ICalculadoraMulta calculadoraMulta)
    {
        this.notificadores = notificadores;
        this.livroService = livroService;
        this.usuarioService = usuarioService;
        this.calculadoraMulta = calculadoraMulta;
    }

    public bool RealizarEmprestimo(int usuarioId, string isbn, int diasEmprestimo)
    {
        var livro = livroService.BuscarPorISBN(isbn);
        var usuario = usuarioService.BuscarPorId(usuarioId);

        if (livro == null || usuario == null || !livro.Disponivel)
            return false;

        livro.Disponivel = false;
        var emprestimo = new Emprestimo
        {
            Livro = livro,
            Usuario = usuario,
            DataEmprestimo = DateTime.Now,
            DataDevolucaoPrevista = DateTime.Now.AddDays(diasEmprestimo)
        };

        emprestimos.Add(emprestimo);
        Notificar(usuario.Nome, "Empréstimo Realizado", $"Você pegou emprestado: {livro.Titulo}");
        return true;
    }

    public double RealizarDevolucao(int usuarioId, string isbn)
    {
        var emprestimo = emprestimos.FirstOrDefault(e => e.Usuario.ID == usuarioId && e.Livro.ISBN == isbn && e.DataDevolucaoEfetiva == null);
        if (emprestimo == null) return -1;

        emprestimo.DataDevolucaoEfetiva = DateTime.Now;
        emprestimo.Livro.Disponivel = true;
        double multa = calculadoraMulta.CalcularMulta(emprestimo.DataDevolucaoPrevista, DateTime.Now);

        if (multa > 0)
            Notificar(emprestimo.Usuario.Nome, "Multa por Atraso", $"Você tem uma multa de R$ {multa}");

        return multa;
    }

    public List<Emprestimo> ListarEmprestimos() => emprestimos;

    private void Notificar(string destinatario, string assunto, string mensagem)
    {
        foreach (var notificador in notificadores)
            notificador.Notificar(destinatario, assunto, mensagem);
    }
}

class Program
{
    static void Main(string[] args)
    {
        var notificadores = new List<INotificador> { new EmailNotificador(), new SMSNotificador() };
        var livroService = new LivroService();
        var usuarioService = new UsuarioService(notificadores);
        var emprestimoService = new EmprestimoService(notificadores, livroService, usuarioService, new CalculadoraMulta());

        livroService.AdicionarLivro("Clean Code", "Robert C. Martin", "978-0132350884");
        usuarioService.AdicionarUsuario("João Silva", 1);

        emprestimoService.RealizarEmprestimo(1, "978-0132350884", 7);

        double multa = emprestimoService.RealizarDevolucao(1, "978-0132350884");
        Console.WriteLine($"Multa: R$ {multa}");
    }
}
