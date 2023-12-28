﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
public class Program
{
    private static List<Catraca> estoqueCatracas = new List<Catraca>();
    private static List<Painel> estoquePaineis = new List<Painel>();
    private static List<Canopla> estoqueCanoplas = new List<Canopla>();

    public static void Main()
    {
        while (true)
        {
            Console.Clear();
            MostrarMenuPrincipal();
            Console.Write("Escolha uma opção (ou 0 para sair): ");
            var opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    MenuEstoqueCatracas();
                    break;
                case "2":
                    MenuEstoquePaineis();
                    break;
                case "3":
                    MenuEstoqueCanoplas();
                    break;
                case "0":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }

            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
        }
    }
    private static void MostrarMenuPrincipal()
    {
        Console.WriteLine("### Menu Principal ###");
        Console.WriteLine("1. Estoque de Catracas");
        Console.WriteLine("2. Estoque de Paineis");
        Console.WriteLine("3. Estoque de Canoplas");
        Console.WriteLine("0. Sair");
    }
    private static void MenuEstoqueCatracas()
    {
        while (true)
        {
            Console.Clear();
            MostrarSubMenuEstoqueCatracas();
            Console.Write("Escolha uma opção (ou 0 para voltar ao Menu Principal): ");
            var opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    ExibirEstoqueCatracas();
                    break;
                case "2":
                    AdicionarCatracaEstoque();
                    break;
                case "3":
                    PreencherInformacoesAdicionaisEmCatracaExistente();
                    break;
                case "4":
                    BuscarPorCodigoCatraca();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }

            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
        }
    }
    // submenu estoque de catracas
    private static void MostrarSubMenuEstoqueCatracas()
    {
        Console.WriteLine("### Estoque de Catracas ###");
        Console.WriteLine("1. Exibir Estoque");
        Console.WriteLine("2. Adicionar Catraca");
        Console.WriteLine("3. Preencher Informações em Catraca Existente");
        Console.WriteLine("4. Buscar por Código");
        Console.WriteLine("0. Voltar ao Menu Principal");
    }
    // métodos do Submenu "Estoque de catracas"
    private static void ExibirEstoqueCatracas()
    {
        Console.WriteLine("\n### Estoque de Catracas ###");

        if (estoqueCatracas.Count == 0)
        {
            Console.WriteLine("Não há catracas no estoque.");
            Console.Write("Deseja adicionar uma catraca? (S/N): ");
            string resposta = Console.ReadLine();

            if (resposta.Trim().ToUpper() == "S")
            {
                AdicionarCatracaEstoque();
            }

            return;
        }

        Console.WriteLine("{0,-15} {1,-20} {2,-20} {3,-15} {4,-15} {5,-20} {6,-15} {7,-15} {8,-25}",
            "Número", "Modelo do Painel", "Marca do Painel", "ID Cliente", "Nota Fiscal", "Transportadora", "Entrada", "Saída", "Código");

        foreach (var catraca in estoqueCatracas)
        {
            Console.WriteLine("{0,-15} {1,-20} {2,-20} {3,-15} {4,-15} {5,-20} {6,-15} {7,-15} {8,-25}",
                catraca.NumeroCatraca,
                GetModeloPainel(catraca.ModeloPainelCorrespondente),
                GetMarcaPainel(catraca.MarcaPainelCorrespondente),
                catraca.IdCliente,
                catraca.NF,
                catraca.Transportadora,
                catraca.Entrada.ToString("dd/MM/yy"),
                catraca.Saida?.ToString("dd/MM/yy") ?? "-",
                catraca.CodigoCatraca);
        }
    

    Console.Write("Deseja adicionar uma catraca? (S/N): ");
        string respostaAdicao = Console.ReadLine();

        if (respostaAdicao.Trim().ToUpper() == "S")
        {
            AdicionarCatracaEstoque();
        }
    }




    private static void AdicionarCatracaEstoque()
    {
        var catraca = new Catraca();

        Console.Write("Digite o Número do Painel: ");
        int numeroPainel = int.Parse(Console.ReadLine());
        catraca.NumeroPainel = numeroPainel.ToString();

        // Buscar o painel no estoque pelo número informado
        var painelCorrespondente = estoquePaineis.FirstOrDefault(p => p.NumeroPainel == numeroPainel);

        if (painelCorrespondente != null)
        {
            // Atualizar para atribuir automaticamente a marca e o modelo do painel
            catraca.ModeloPainelCorrespondente = painelCorrespondente.ModeloPainel;
            catraca.MarcaPainelCorrespondente = painelCorrespondente.Marca;

            // Solicitar obrigatoriamente o número da Canopla
            Console.Write("Digite o Número da Canopla: ");
            catraca.Canopla = Console.ReadLine();
            

            PreencherInformacoesCatraca(catraca, painelCorrespondente.ModeloPainel, painelCorrespondente.Marca);

            catraca.CodigoCatraca = Catraca.GerarCodigoCatraca(catraca);
            estoqueCatracas.Add(catraca);
            Console.WriteLine("\nCatraca adicionada ao estoque com sucesso!");
        }
        else
        {
            Console.WriteLine("Painel não encontrado. Verifique o número do painel e tente novamente.");
        }
    }
   
    private static void PreencherInformacoesCatraca(Catraca catraca, ModeloPainel modeloPainel, MarcaPainel marcaPainel)
    {
        Console.Write("Digite o Número da Catraca: ");
        catraca.NumeroCatraca = int.Parse(Console.ReadLine());

        // Mapeamento do ModeloPainel para ModeloCatraca
        catraca.Modelo = GetModeloCatraca(modeloPainel);

        // Atribuir automaticamente a marca do painel
        catraca.Marca = GetMarcaCatraca(marcaPainel);

        Console.Write("Digite o ID do Cliente: ");
        catraca.IdCliente = Console.ReadLine();

        Console.Write("Digite a NF: ");
        catraca.NF = Console.ReadLine();

        // Solicitar a Transportadora se a NF não estiver vazia
        if (!string.IsNullOrWhiteSpace(catraca.NF))
        {
            Console.Write("Digite a Transportadora: ");
            catraca.Transportadora = Console.ReadLine();
        }

        // **Armazenar a data de entrada corretamente no objeto catraca**
        Console.Write("Digite a Data de Entrada : ");
        string entradaInput = Console.ReadLine();

        // Continuar pedindo a data até que uma entrada válida seja fornecida
        while (string.IsNullOrWhiteSpace(entradaInput) || !DateTime.TryParseExact(entradaInput, new[] { "dd/MM/yy", "dd/MM/yyyy" }, null, DateTimeStyles.None, out var entrada))
        {
            Console.WriteLine("Formato de data inválido. Por favor, forneça uma data válida.");
            Console.Write("Digite a Data de Entrada : ");
            entradaInput = Console.ReadLine();
        }

        catraca.Entrada = DateTime.ParseExact(entradaInput, new[] { "dd/MM/yy", "dd/MM/yyyy" }, null, DateTimeStyles.None);

        Console.Write("Responsável Catraca: ");
        catraca.ResponsavelCatraca = Console.ReadLine();

        // Verificar se o nome é válido (não está vazio)
        while (string.IsNullOrWhiteSpace(catraca.ResponsavelCatraca))
        {
            Console.WriteLine("Por favor, digite o nome do responsável.");
            Console.Write("Responsável Catraca: ");
            catraca.ResponsavelCatraca = Console.ReadLine();
        }

    }
    private static void PreencherInformacoesAdicionaisEmCatracaExistente()
    {
        Console.Write("Digite o número da catraca: ");
        int numeroCatraca = int.Parse(Console.ReadLine());

        var catracaExistente = estoqueCatracas.FirstOrDefault(catraca => catraca.NumeroCatraca == numeroCatraca);

        if (catracaExistente != null)
        {
            // Preencher apenas as informações adicionais que faltam
            if (string.IsNullOrWhiteSpace(catracaExistente.NF) || string.IsNullOrWhiteSpace(catracaExistente.Transportadora) || catracaExistente.Saida == null)
            {
                PreencherInformacoesAdicionais(catracaExistente);
            }

            Console.WriteLine("\nInformações adicionais preenchidas com sucesso!");
        }
        else
        {
            Console.WriteLine("\nCatraca não encontrada no estoque.");
        }
    }
    private static void PreencherInformacoesAdicionais(Catraca catraca)
    {
        // Verificar se a NF está vazia para decidir se deve solicitar o ID do Cliente e a Transportadora
        if (string.IsNullOrWhiteSpace(catraca.NF))
        {
            Console.Write("Digite a NF: ");
            catraca.NF = Console.ReadLine();

            // Ao adicionar NF, o ID do Cliente será obrigatório
            Console.Write("Digite o ID do Cliente: ");
            catraca.IdCliente = Console.ReadLine();

            // Ao adicionar NF, a Transportadora será obrigatória
            Console.Write("Digite a Transportadora: ");
            catraca.Transportadora = Console.ReadLine();
        }

        // Solicitar a Data de Saída se não estiver preenchida
        if (!catraca.Saida.HasValue)
        {
            Console.Write("Digite a Data de Saída (ou deixe em branco se não houver): ");
            var dataSaidaInput = Console.ReadLine();

            DateTime? dataSaida = null;

            if (!string.IsNullOrWhiteSpace(dataSaidaInput))
            {
                // Tenta analisar a data no formato DD/MM/AA ou DD/MM/AAAA
                if (DateTime.TryParseExact(dataSaidaInput, new[] { "dd/MM/yy", "dd/MM/yyyy" }, null, System.Globalization.DateTimeStyles.None, out var parsedDate))
                {
                    dataSaida = parsedDate;
                }
                else
                {
                    Console.WriteLine("Formato de data inválido. Por favor, insira a data no formato DD/MM/AA ou DD/MM/AAAA.");
                    // Você pode optar por pedir a entrada novamente ou tomar outra ação apropriada aqui
                }
            }

            catraca.Saida = dataSaida;
        }
    }
    private static ModeloCatraca GetModeloCatraca(ModeloPainel modeloPainel)
    {
        switch (modeloPainel)
        {
            case ModeloPainel.Embarcado:
                return ModeloCatraca.Embarcado;
            case ModeloPainel.DigitalPersona:
                return ModeloCatraca.DigitalPersona;
            default:
                // Adicione um valor padrão ou lance uma exceção, se apropriado.
                throw new InvalidOperationException("Modelo de catraca não reconhecido.");
        }
    }
    private static MarcaCatraca GetMarcaCatraca(MarcaPainel marcaPainel)
    {
        switch (marcaPainel)
        {
            case MarcaPainel.FacilFit:
                return MarcaCatraca.FacilFit;
            case MarcaPainel.Toletus:
                return MarcaCatraca.Toletus;
            default:
                // Adicione um valor padrão ou lance uma exceção, se apropriado.
                throw new InvalidOperationException("Marca de catraca não reconhecida.");
        }
    }
    private static string GetModelo(ModeloCatraca modeloCatraca, MarcaPainel marcaPainel)
    {
        string modeloCatracaStr = GetModeloCatraca(modeloCatraca);
        string marcaPainelStr = GetMarcaPainel(marcaPainel);

        return $"{modeloCatracaStr} {marcaPainelStr}";
    }
    private static string GetModeloCatraca(ModeloCatraca modeloCatraca)
    {
        switch (modeloCatraca)
        {
            case ModeloCatraca.Embarcado:
                return "Toletus";
            case ModeloCatraca.DigitalPersona:
                return "FacilFit";
            default:
                // Adicione um valor padrão ou lance uma exceção, se apropriado.
                throw new InvalidOperationException("Modelo de catraca não reconhecido.");
        }
    }
    private static void BuscarPorCodigoCatraca()
    {
        Console.Write("Digite o código da catraca: ");
        string codigo = Console.ReadLine();

        var catracaEncontrada = estoqueCatracas.FirstOrDefault(catraca => catraca.CodigoCatraca == codigo);

        if (catracaEncontrada != null)
        {
            Console.WriteLine("\n### Catraca Encontrada ###");
            ExibirDetalhesCatraca(catracaEncontrada);
        }
        else
        {
            Console.WriteLine("\nCatraca não encontrada no estoque.");
        }
    }
    private static void ExibirDetalhesCatraca(Catraca catraca)
    {
        Console.WriteLine("{0,-15} {1,-20} {2,-20} {3,-15} {4,-15} {5,-20} {6,-15} {7,-15} {8,-25}",
           "Número", "Modelo do Painel", "Marca do Painel", "ID Cliente", "Nota Fiscal", "Transportadora", "Entrada", "Saída", "Código");
        {
            Console.WriteLine("{0,-15} {1,-20} {2,-20} {3,-15} {4,-15} {5,-20} {6,-15} {7,-15} {8,-25}",
                catraca.NumeroCatraca,
                GetModeloPainel(catraca.ModeloPainelCorrespondente),
                GetMarcaPainel(catraca.MarcaPainelCorrespondente),
                catraca.IdCliente,
                catraca.NF,
                catraca.Transportadora,
                catraca.Entrada.ToString("dd/MM/yy"),
                catraca.Saida?.ToString("dd/MM/yy") ?? "-",
                catraca.CodigoCatraca);
        }
    }
    //fim do "estoque de catracas"
    //menu estoque de paineis
    private static void MenuEstoquePaineis()
    {
        while (true)
        {
            Console.Clear();
            MostrarSubMenuEstoquePaineis();
            Console.Write("Escolha uma opção (ou 0 para voltar ao Menu Principal): ");
            var opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    ExibirEstoquePaineis();
                    break;
                case "2":
                    AdicionarPainelEstoque();
                    break;
                case "3":
                    BuscarPorCodigoPainel();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }

            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
        }
    }
    private static void MostrarSubMenuEstoquePaineis()
    {
        Console.WriteLine("### Estoque de Paineis ###");
        Console.WriteLine("1. Exibir Estoque");
        Console.WriteLine("2. Adicionar Painel");
        Console.WriteLine("3. Buscar por Código");
        Console.WriteLine("0. Voltar ao Menu Principal");
    }
    private static void ExibirEstoquePaineis()
    {
        Console.WriteLine("\n### Estoque de Paineis Testados ###");

        if (estoquePaineis.Count == 0)
        {
            Console.WriteLine("Não há painéis testados no estoque.");
            return;
        }

        Console.WriteLine("{0,-15} {1,-15} {2,-15} {3,-15} {4,-25} {5,-15}",
            "Número", "Modelo", "Marca", "Data de Teste", "Código", "Responsável");

        foreach (var painelTestado in estoquePaineis)
        {
            Console.WriteLine("{0,-15} {1,-15} {2,-15} {3,-15} {4,-25} {5,-15}",
                painelTestado.NumeroPainel,
                GetModeloPainel(painelTestado.ModeloPainel),
                GetMarcaPainel(painelTestado.Marca),
                painelTestado.DataTeste.ToString("dd/MM/yy"),
                painelTestado.CodigoPainel,
                painelTestado.ResponsavelPainel);
        }
    }
    private static string GetModeloPainel(ModeloPainel modelo)
    {
        switch (modelo)
        {
            case ModeloPainel.DigitalPersona:
                return "DigitalPersona";
            case ModeloPainel.Embarcado:
                return "Embarcado";
            default:
                // Adicione um valor padrão ou lance uma exceção, se apropriado.
                throw new InvalidOperationException("Modelo de painel não reconhecido.");
        }
    }
    private static string GetMarcaPainel(MarcaPainel marca)
    {
        switch (marca)
        {
            case MarcaPainel.FacilFit:
                return "FacilFit";
            case MarcaPainel.Toletus:
                return "Toletus";
            default:
                // Adicione um valor padrão ou lance uma exceção, se apropriado.
                throw new InvalidOperationException("Marca de painel não reconhecido.");
        }
    }
    private static void AdicionarPainelEstoque()
    {
        if (estoquePaineis.Count > 0)
        {
            Console.WriteLine($"Último painel inserido: {estoquePaineis.Last().NumeroPainel}");
        }

        Console.Write("Quantos painéis foram testados? ");
        string quantidadeInput = Console.ReadLine();

        if (!int.TryParse(quantidadeInput, out int quantidade) || quantidade <= 0)
        {
            Console.WriteLine("Quantidade inválida. Operação cancelada.");
            return;
        }

        int numeroPrimeiroPainel;

        do
        {
            Console.Write("Digite o Número do Primeiro Painel: ");
            string input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out numeroPrimeiroPainel))
            {
                break;
            }
            else
            {
                Console.WriteLine("Por favor, digite um número válido.");
            }
        } while (true);

        // Verifica se o número do painel já existe no estoque
        while (estoquePaineis.Any(p => p.NumeroPainel == numeroPrimeiroPainel))
        {
            Console.WriteLine($"Já existe um painel com o número {numeroPrimeiroPainel}. Escolha outro número.");
            Console.Write("Digite o Número do Primeiro Painel: ");
            numeroPrimeiroPainel = int.Parse(Console.ReadLine());
        }

        var painelTestado = new Painel();
        Console.WriteLine($"\n### Informações do Painel {numeroPrimeiroPainel} ###");

        // Escolha do Modelo do Painel
        int modeloIndex;
        do
        {
            Console.WriteLine("Escolha o Modelo do Painel:");
            Console.WriteLine("1. DigitalPersona");
            Console.WriteLine("2. Embarcado");
            Console.Write("Digite o número correspondente ao Modelo: ");
            string modeloInput = Console.ReadLine();

            if (int.TryParse(modeloInput, out modeloIndex) && (modeloIndex == 1 || modeloIndex == 2))
            {
                break;
            }
            else
            {
                Console.WriteLine("Por favor, escolha um número válido para o Modelo.");
            }
        } while (true);

        painelTestado.ModeloPainel = (ModeloPainel)(modeloIndex - 1); // Ajuste para o índice do enum

        // Escolha da Marca do Painel
        int marcaIndex;
        do
        {
            Console.WriteLine("Escolha a Marca do Painel:");
            Console.WriteLine("1. FacilFit");
            Console.WriteLine("2. Toletus");
            Console.Write("Digite o número correspondente à Marca: ");
            string marcaInput = Console.ReadLine();

            if (int.TryParse(marcaInput, out marcaIndex) && (marcaIndex == 1 || marcaIndex == 2))
            {
                break;
            }
            else
            {
                Console.WriteLine("Por favor, escolha um número válido para a Marca.");
            }
        } while (true);

        painelTestado.Marca = (MarcaPainel)(marcaIndex - 1); // Ajuste para o índice do enum

        DateTime dataTeste;

        do
        {
            Console.Write("Digite a Data do Teste: ");
            string dataTesteInput = Console.ReadLine();

            if (DateTime.TryParseExact(dataTesteInput, new[] { "dd/MM/yyyy", "dd/MM/yy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out dataTeste))
            {
                // A data foi inserida corretamente, você pode atribuir à propriedade desejada
                painelTestado.DataTeste = dataTeste;
                break; // Sai do loop, pois a data é válida
            }
            else
            {
                Console.WriteLine("Data inválida. Por favor, insira uma data no formato dd/MM/yyyy ou dd/MM/yy.");
            }
        } while (true);


        string responsavel;

        do
        {
            Console.Write("Digite o Responsável pelo Teste: ");
            responsavel = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(responsavel))
            {
                // O nome do responsável foi inserido corretamente
                painelTestado.ResponsavelPainel = responsavel;
                break; // Sai do loop, pois o nome do responsável é válido
            }
            else
            {
                Console.WriteLine("O nome do responsável é obrigatório. Por favor, informe novamente.");
            }
        } while (true);


        for (int i = 0; i < quantidade; i++)
        {
            var novoPainel = new Painel
            {
                NumeroPainel = numeroPrimeiroPainel + i,
                ModeloPainel = painelTestado.ModeloPainel,
                Marca = painelTestado.Marca,
                DataTeste = painelTestado.DataTeste,
                ResponsavelPainel = painelTestado.ResponsavelPainel,
                CodigoPainel = $"PA{numeroPrimeiroPainel + i}{painelTestado.ResponsavelPainel.Substring(0, 2)}",

            };

            estoquePaineis.Add(novoPainel);
            string codigoPainel = $"pa{numeroPrimeiroPainel + i}{painelTestado.ResponsavelPainel.Substring(0, 2)}";
            novoPainel.CodigoPainel = codigoPainel;
        }

        Console.WriteLine($"{quantidade} painéis adicionados ao estoque. Último painel adicionado: {estoquePaineis.Last().NumeroPainel}");
    }
    private static void BuscarPorCodigoPainel()
    {
        Console.Write("Digite o código do painel: ");
        string codigoBusca = Console.ReadLine();

        var painelEncontrado = estoquePaineis.FirstOrDefault(p => p.CodigoPainel == codigoBusca);

        if (painelEncontrado != null)
        {
            Console.WriteLine("\n### Painel Encontrado ###");
            Console.WriteLine($"Número do Painel Testado: {painelEncontrado.NumeroPainel}");
            Console.WriteLine($"Modelo do Painel: {GetModelo(painelEncontrado.ModeloPainel)}");
            Console.WriteLine($"Marca do Painel: {GetMarca(painelEncontrado.Marca)}");
            Console.WriteLine($"Data do Teste: {painelEncontrado.DataTeste.ToString("dd/MM/yy")}");
            Console.WriteLine($"Código do Painel Testado: {painelEncontrado.CodigoPainel}");
        }
        else
        {
            Console.WriteLine("Painel não encontrado. Verifique o código e tente novamente.");
        }
    }
    //fim do menu estoque de paineis
    //menu estoque de canopla
    private static void MenuEstoqueCanoplas()
    {
        while (true)
        {
            Console.Clear();
            MostrarSubMenuEstoqueCanoplas();
            Console.Write("Escolha uma opção (ou 0 para voltar ao Menu Principal): ");
            var opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    ExibirEstoqueCanoplas();
                    break;
                case "2":
                    AdicionarCanoplaEstoque();
                    break;
                case "3":
                    BuscarPorCodigoCanopla();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }

            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
        }
    }
    private static void MostrarSubMenuEstoqueCanoplas()
    {
        Console.WriteLine("### Estoque de Canoplas ###");
        Console.WriteLine("1. Exibir Estoque");
        Console.WriteLine("2. Adicionar Canopla");
        Console.WriteLine("3. Buscar por Código");
        Console.WriteLine("0. Voltar ao Menu Principal");
    }
    private static void ExibirEstoqueCanoplas()
    {
        Console.WriteLine("\n### Estoque de Canoplas ###");

        if (estoqueCanoplas.Count == 0)
        {
            Console.WriteLine("Não há canoplas no estoque.");
            return;
        }

        Console.WriteLine("{0,-15} {1,-15} {2,-15} {3,-25}",
            "Número", "Data de Entrada", "Responsável", "Código");

        foreach (var canopla in estoqueCanoplas)
        {
            Console.WriteLine("{0,-15} {1,-15} {2,-15} {3,-25}",
                canopla.NumeroCanopla,
                canopla.DataEntrada.ToString("dd/MM/yy"),
                canopla.ResponsavelCanopla,
                canopla.CodigoCanopla);
        }
    }
    private static void AdicionarCanoplaEstoque()
    {
        Console.Write("Quantas canoplas serão adicionadas? (Pressione 'X' para cancelar): ");
        string quantidadeInput = Console.ReadLine();

        if (quantidadeInput.Equals("X", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Operação cancelada.");
            return;
        }

        if (!int.TryParse(quantidadeInput, out int quantidade) || quantidade <= 0)
        {
            Console.WriteLine("Quantidade inválida. Operação cancelada.");
            return;
        }

        int proximoNumeroCanopla = estoqueCanoplas.Count > 0 ? estoqueCanoplas.Max(c => c.NumeroCanopla) + 1 : 1;

        Console.Write("\nData de Entrada no Estoque: ");
        string dataEntradaInput = Console.ReadLine();

        DateTimeStyles dateTimeStyles = DateTimeStyles.None;
        string[] formats = { "dd/MM/yy", "dd/MM/yyyy" };

        if (!DateTime.TryParseExact(dataEntradaInput, formats, null, dateTimeStyles, out DateTime dataEntrada))
        {
            Console.WriteLine("Data inválida. Operação cancelada.");
            return;
        }


        Console.Write("Nome do Responsável pela Canopla: ");
        string responsavelCanopla = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(responsavelCanopla))
        {
            Console.WriteLine("Nome do responsável inválido. Operação cancelada.");
            return;
        }

        for (int i = 0; i < quantidade; i++)
        {
            // Verificar se o número da canopla já existe
            while (estoqueCanoplas.Any(c => c.NumeroCanopla == proximoNumeroCanopla))
            {
                proximoNumeroCanopla++;
            }

            // Outras informações específicas da canopla, se houver

            Canopla novaCanopla = new Canopla
            {
                NumeroCanopla = proximoNumeroCanopla,
                DataEntrada = dataEntrada,
                ResponsavelCanopla = responsavelCanopla,
                // Atribua outras informações específicas da canopla, se houver
            };

            // Gerar código da canopla
            string codigoCanopla = $"CAN{proximoNumeroCanopla}{responsavelCanopla.Substring(0, Math.Min(2, responsavelCanopla.Length))}";
            novaCanopla.CodigoCanopla = codigoCanopla;

            estoqueCanoplas.Add(novaCanopla);

            Console.WriteLine($"Canopla {proximoNumeroCanopla} adicionada ao estoque. Código: {codigoCanopla}");

            proximoNumeroCanopla++;
        }

        Console.WriteLine($"{quantidade} canoplas adicionadas ao estoque. Última canopla adicionada: {estoqueCanoplas.Last().NumeroCanopla}");
    }
    private static void BuscarPorCodigoCanopla()
    {
        Console.Write("Digite o código da canopla: ");
        string codigoCanopla = Console.ReadLine();

        var canoplaEncontrada = estoqueCanoplas.FirstOrDefault(c => c.CodigoCanopla == codigoCanopla);

        if (canoplaEncontrada != null)
        {
            Console.WriteLine("\n### Informações da Canopla Encontrada ###");
            Console.WriteLine($"Número da Canopla: {canoplaEncontrada.NumeroCanopla}");
            Console.WriteLine($"Data de Entrada: {canoplaEncontrada.DataEntrada.ToString("dd/MM/yy")}");
            Console.WriteLine($"Responsável: {canoplaEncontrada.ResponsavelCanopla}");
            Console.WriteLine($"Código: {canoplaEncontrada.CodigoCanopla}");
            // Adicione outras informações específicas da canopla, se houver
        }
        else
        {
            Console.WriteLine("Canopla não encontrada. Verifique o código informado.");
        }
    }
    private static string GetModelo(ModeloPainel modelo)
    {
        switch (modelo)
        {
            case ModeloPainel.Embarcado:
                return "EMBARCADA";
            case ModeloPainel.DigitalPersona:
                return "DIGITAL PERSONA";
            default:
                return "Desconhecido";
        }
    }
    private static string GetMarca(MarcaPainel marca)
    {
        switch (marca)
        {
            case MarcaPainel.FacilFit:
                return "FacilFit";
            case MarcaPainel.Toletus:
                return "Toletus";
            default:
                return "Desconhecida";
        }
    }
    public enum ModeloCatraca
    {
        DigitalPersona,
        Embarcado
    }
    public enum MarcaCatraca
    {
        FacilFit,
        Toletus
    }
    public enum ModeloPainel
    {
        DigitalPersona,
        Embarcado
    }
    public enum MarcaPainel
    {
        FacilFit,
        Toletus
    }
    public class Painel
    {
        public int NumeroPainel { get; set; }
        public ModeloPainel ModeloPainel { get; set; }
        public MarcaPainel Marca { get; set; }
        public DateTime DataTeste { get; set; }
        public string ResponsavelPainel { get; set; }

        private string codigoPainel; // Armazenar o valor real da propriedade

        public string CodigoPainel
        {
            get { return $"PA{NumeroPainel}{ResponsavelPainel.Substring(0, 2)}"; }
            set { codigoPainel = value; } // Permitir a atribuição
        }
    }
    public class Canopla
    {
        public int NumeroCanopla { get; set; }
        public DateTime DataEntrada { get; set; }
        public string ResponsavelCanopla { get; set; }
        public string CodigoCanopla { get; set; } 
    }
    public class Catraca
    {
        public int NumeroCatraca { get; set; }  
        public ModeloCatraca Modelo { get; set; }
        public MarcaCatraca Marca { get; set; }
       
        public string IdCliente { get; set; }
        public string NF { get; set; }
        public string Transportadora { get; set; }
        public DateTime Entrada { get; set; }
        public DateTime? Saida { get; set; }
        private string codigoCatraca; // Armazenar o valor real da propriedade
        public ModeloPainel ModeloPainelCorrespondente { get; set; }
        public MarcaPainel MarcaPainelCorrespondente { get; set; }

        public string CodigoCatraca
        {
            get { return GerarCodigoCatraca(this); }
            set { codigoCatraca = value; } // Permitir a atribuição
        }
        public string Canopla { get; set; }
        public string ResponsavelCatraca { get; set; }
        public string NumeroPainel { get; set; }
        public string NumeroCanopla { get; set; }
        public static string GerarCodigoCatraca(Catraca catraca)
        {
            string numeroCatraca = catraca.NumeroCatraca.ToString();
            string duasLetrasResponsavel = catraca.ResponsavelCatraca.Substring(0, 2).ToUpper(); // Garantindo letras maiúsculas
            string numeroPainel = catraca.NumeroPainel; 
            string numeroCanopla = catraca.Canopla; 

            return $"C{numeroCatraca}{duasLetrasResponsavel}-{numeroPainel}-{numeroCanopla}";
        }

    }




}



