using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        SistemaClinico sistema = new SistemaClinico(10);
        while (true)
        {
            Console.WriteLine("\n===== MENU SISTEMA CLÍNICO =====");
            Console.WriteLine("1. Cadastrar Paciente");
            Console.WriteLine("2. Buscar Paciente");
            Console.WriteLine("3. Atualizar Dados Clínicos");
            Console.WriteLine("4. Remover Paciente");
            Console.WriteLine("5. Exibir Tabela Hash");
            Console.WriteLine("6. Exibir Fila de Triagem");
            Console.WriteLine("7. Atender Paciente");
            Console.WriteLine("8. Exibir Histórico de Atendimentos");
            Console.WriteLine("0. Sair");
            Console.Write("Escolha uma opção: ");

            switch (Console.ReadLine())
            {
                case "1": sistema.CadastrarPaciente(); break;
                case "2": sistema.BuscarPaciente(); break;
                case "3": sistema.AtualizarPaciente(); break;
                case "4": sistema.RemoverPaciente(); break;
                case "5": sistema.ExibirTabela(); break;
                case "6": sistema.ExibirFilaTriagem(); break;
                case "7": sistema.AtenderPaciente(); break;
                case "8": sistema.ExibirHistorico(); break;
                case "0": return;
                default: Console.WriteLine("Opção inválida."); break;
            }
        }
    }
}

enum Prioridade
{
    Verde,
    Amarela,
    Vermelha
}

class Paciente
{
    public string CPF { get; set; }
    public string Nome { get; set; }
    public double PressaoArterial { get; set; }
    public double Temperatura { get; set; }
    public double Oxigenacao { get; set; }
    public Prioridade PrioridadeAtendimento { get; private set; }

    public Paciente(string cpf, string nome, double pa, double temp, double oxi)
    {
        CPF = cpf;
        Nome = nome;
        PressaoArterial = pa;
        Temperatura = temp;
        Oxigenacao = oxi;
        AtualizarPrioridade();
    }

    public void AtualizarDados(double pa, double temp, double oxi)
    {
        PressaoArterial = pa;
        Temperatura = temp;
        Oxigenacao = oxi;
        AtualizarPrioridade();
    }

    private void AtualizarPrioridade()
    {
        if (PressaoArterial > 18 || Temperatura > 39 || Oxigenacao < 90)
            PrioridadeAtendimento = Prioridade.Vermelha;
        else if (PressaoArterial < 10 || Temperatura > 37.5 || Temperatura < 36 || Oxigenacao < 95)
            PrioridadeAtendimento = Prioridade.Amarela;
        else
            PrioridadeAtendimento = Prioridade.Verde;
    }

    public override string ToString()
    {
        ConsoleColor cor = ConsoleColor.Green;
        switch (PrioridadeAtendimento)
        {
            case Prioridade.Vermelha: cor = ConsoleColor.Red; break;
            case Prioridade.Amarela: cor = ConsoleColor.Yellow; break;
            case Prioridade.Verde: cor = ConsoleColor.Green; break;
        }

        Console.ForegroundColor = cor;
        string texto = $"CPF: {CPF}, Nome: {Nome}, PA: {PressaoArterial}, Temp: {Temperatura}, O₂: {Oxigenacao}, Prioridade: {PrioridadeAtendimento}";
        Console.ResetColor();
        return texto;
    }
}

class TabelaHash
{
    private int capacidade;
    private LinkedList<Paciente>[] buckets;

    public TabelaHash(int capacidade)
    {
        this.capacidade = capacidade;
        buckets = new LinkedList<Paciente>[capacidade];
        for (int i = 0; i < capacidade; i++)
            buckets[i] = new LinkedList<Paciente>();
    }

    private int FuncaoHash(string chave)
    {
        return Math.Abs(chave.GetHashCode()) % capacidade;
    }

    public void Inserir(Paciente paciente)
    {
        int indice = FuncaoHash(paciente.CPF);
        foreach (var p in buckets[indice])
        {
            if (p.CPF == paciente.CPF)
            {
                Console.WriteLine("CPF já cadastrado.");
                return;
            }
        }
        buckets[indice].AddLast(paciente);
        Console.WriteLine("Paciente cadastrado com sucesso.");
    }

    public Paciente Buscar(string cpf)
    {
        int indice = FuncaoHash(cpf);
        foreach (var paciente in buckets[indice])
        {
            if (paciente.CPF == cpf)
                return paciente;
        }
        return null;
    }

    public void Atualizar(string cpf, double pa, double temp, double oxi)
    {
        var paciente = Buscar(cpf);
        if (paciente != null)
        {
            paciente.AtualizarDados(pa, temp, oxi);
            Console.WriteLine("Dados atualizados.");
        }
        else
            Console.WriteLine("Paciente não encontrado.");
    }

    public void Remover(string cpf)
    {
        int indice = FuncaoHash(cpf);
        var bucket = buckets[indice];
        foreach (var paciente in bucket)
        {
            if (paciente.CPF == cpf)
            {
                bucket.Remove(paciente);
                Console.WriteLine("Paciente removido.");
                return;
            }
        }
        Console.WriteLine("Paciente não encontrado.");
    }

    public void ExibirTabela()
    {
        for (int i = 0; i < capacidade; i++)
        {
            Console.WriteLine($"Bucket {i}:");
            foreach (var paciente in buckets[i])
            {
                Console.WriteLine("  " + paciente);
            }
        }
    }
}

class SistemaClinico
{
    private TabelaHash tabela;
    private Queue<Paciente> filaTriagem;
    private Stack<Paciente> historicoAtendimentos;

    public SistemaClinico(int capacidadeHash)
    {
        tabela = new TabelaHash(capacidadeHash);
        filaTriagem = new Queue<Paciente>();
        historicoAtendimentos = new Stack<Paciente>();
    }

    public void CadastrarPaciente()
    {
        Console.Write("CPF: "); string cpf = Console.ReadLine();
        Console.Write("Nome: "); string nome = Console.ReadLine();

        double pa = LerDouble("Pressão Arterial: ");
        double temp = LerDouble("Temperatura: ");
        double oxi = LerDouble("Oxigenação: ");

        var paciente = new Paciente(cpf, nome, pa, temp, oxi);
        tabela.Inserir(paciente);
        filaTriagem.Enqueue(paciente);
    }

    public void BuscarPaciente()
    {
        Console.Write("Digite o CPF: ");
        string cpf = Console.ReadLine();
        var paciente = tabela.Buscar(cpf);
        Console.WriteLine(paciente != null ? paciente.ToString() : "Paciente não encontrado.");
    }

    public void AtualizarPaciente()
    {
        Console.Write("CPF: ");
        string cpf = Console.ReadLine();

        double pa = LerDouble("Nova Pressão Arterial: ");
        double temp = LerDouble("Nova Temperatura: ");
        double oxi = LerDouble("Nova Oxigenação: ");

        tabela.Atualizar(cpf, pa, temp, oxi);
    }

    public void RemoverPaciente()
    {
        Console.Write("Digite o CPF: ");
        string cpf = Console.ReadLine();
        tabela.Remover(cpf);
    }

    public void ExibirTabela()
    {
        tabela.ExibirTabela();
    }

    public void ExibirFilaTriagem()
    {
        Console.WriteLine("Fila de Triagem:");
        foreach (var paciente in filaTriagem)
            Console.WriteLine(paciente);
    }

    public void AtenderPaciente()
    {
        if (filaTriagem.Count == 0)
        {
            Console.WriteLine("Nenhum paciente na fila.");
            return;
        }

        var paciente = filaTriagem.Dequeue();
        historicoAtendimentos.Push(paciente);
        Console.WriteLine("Paciente atendido:");
        Console.WriteLine(paciente);
    }

    public void ExibirHistorico()
    {
        if (historicoAtendimentos.Count == 0)
        {
            Console.WriteLine("Nenhum atendimento realizado ainda.");
            return;
        }

        Console.WriteLine("Histórico de Atendimentos (último → primeiro):");
        foreach (var paciente in historicoAtendimentos)
            Console.WriteLine(paciente);
    }

    private double LerDouble(string mensagem)
    {
        double valor;
        Console.Write(mensagem);
        while (!double.TryParse(Console.ReadLine(), out valor))
        {
            Console.WriteLine("Valor inválido, tente novamente.");
            Console.Write(mensagem);
        }
        return valor;
    }
}
