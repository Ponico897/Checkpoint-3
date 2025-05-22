# Checkpoint-3
Atividade de aplicações orientadas a objetos.

1. Violação do SRP (Single Responsibility Principle)
Local: GerenciadorBiblioteca – vários métodos
Explicação: A classe faz muitas coisas: gerencia livros, usuários, empréstimos, cálculo de multas e envio de notificações.
Por que é ruim: Isso torna a classe difícil de manter, testar e estender. Alterações em uma responsabilidade podem impactar as outras.

2. Violação do OCP (Open/Closed Principle)
Local: GerenciadorBiblioteca.RealizarEmprestimo
Explicação: O método envia notificações por e-mail e SMS diretamente, o que exige modificações na classe se um novo tipo de notificação for adicionado.
Por que é ruim: Viola o princípio de estar fechado para modificação, pois qualquer nova forma de notificação exigirá alterações nesta classe.

3. Violação do DIP (Dependency Inversion Principle)
Local: GerenciadorBiblioteca depende diretamente dos métodos EnviarEmail e EnviarSMS
Explicação: A classe depende de implementações concretas de envio de mensagens.
Por que é ruim: Dificulta testes e impede a inversão de controle via injeção de dependência.

4. Violação do Clean Code (Funções com múltiplas responsabilidades)
Local: RealizarEmprestimo e RealizarDevolucao
Explicação: Ambos fazem lógica de negócio e envio de notificações.
Por que é ruim: Mistura de responsabilidades diminui legibilidade e aumenta o acoplamento.

5. Violação do ISP (Interface Segregation Principle)
Local: Não há interfaces específicas para serviços como envio de mensagens.
Explicação: Todos os tipos de notificação poderiam ser tratados por uma interface segregada.
Por que é ruim: Dificulta reutilização e implementação independente de partes específicas do sistema.
