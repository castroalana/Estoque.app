using Firebase.Database;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.Globalization;
using Firebase.Database.Query;


public class Program
{

    private static List<Painel> estoquePaineis = new List<Painel>();
    private static List<Canopla> estoqueCanoplas = new List<Canopla>();
   

    public static void Main()
    {
        //configuração firebase
        var cred = GoogleCredential.FromFile(@"C:\Users\Acpast\Desktop\PROJETO CONTROLE PRODUÇÃO\ESTOQUE TOLETUS\CORRIGIDO\ESTOQUE\estoque-4d5c2-firebase-adminsdk-h07rp-cb1daad088.json");

        string path = @"C:\Users\Acpast\Desktop\PROJETO CONTROLE PRODUÇÃO\ESTOQUE TOLETUS\CORRIGIDO\ESTOQUE\estoque-4d5c2-firebase-adminsdk-h07rp-cb1daad088.json";
        string json = File.ReadAllText(path);

        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromJson(json),


        });
        var firebaseClient = new FirebaseClient("https://estoque-4d5c2-default-rtdb.firebaseio.com/");


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
                case "4":
                    MenuExcluirPeca(firebaseClient);
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
        Console.WriteLine("4. Excluir do Estoque");     
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

        // Recuperar catracas do Firebase
        var novoFirebaseClient = new FirebaseClient("https://estoque-4d5c2-default-rtdb.firebaseio.com/");
        var estoqueCatracasFirebase = novoFirebaseClient.Child("estoqueCatracas").OnceAsync<Catraca>().Result.Select(item => item.Object).ToList();

        if (estoqueCatracasFirebase.Count == 0)
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

        foreach (var catraca in estoqueCatracasFirebase)
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
        // Declaração das variáveis
        var catraca = new Catraca();
        var estoqueCatracasFirebase = new List<Catraca>();
        var novoFirebaseClient = new FirebaseClient("https://estoque-4d5c2-default-rtdb.firebaseio.com/");

        // Obter o número do painel
        int numeroPainel;
        do
        {
            Console.Write("Digite o Número do Painel: ");
            numeroPainel = int.Parse(Console.ReadLine());
            catraca.NumeroPainel = numeroPainel.ToString();

            // Buscar o painel no estoque pelo número informado
            var estoquePaineisFirebase = novoFirebaseClient.Child("estoquePaineis").OnceAsync<Painel>().Result.Select(item => item.Object).ToList();
            var painelCorrespondente = estoquePaineisFirebase.FirstOrDefault(p => p.NumeroPainel == numeroPainel);

            // Verificar se o painel existe
            if (painelCorrespondente != null)
            {
                // Atualizar para atribuir automaticamente a marca e o modelo do painel
                catraca.ModeloPainelCorrespondente = painelCorrespondente.ModeloPainel;
                catraca.MarcaPainelCorrespondente = painelCorrespondente.Marca;

                // Obter o estoque de catracas do Firebase
                estoqueCatracasFirebase = novoFirebaseClient.Child("estoqueCatracas").OnceAsync<Catraca>().Result.Select(item => item.Object).ToList();

                // Verificar se o número do painel já foi utilizado em outra catraca
                var catracaComMesmoNumeroPainel = estoqueCatracasFirebase.FirstOrDefault(c => c.NumeroPainel == catraca.NumeroPainel);

                if (catracaComMesmoNumeroPainel != null)
                {
                    Console.WriteLine($"PAINEL JÁ UTILIZADO NA CATRACA ({catracaComMesmoNumeroPainel.NumeroCatraca}) - INSIRA OUTRO PAINEL");
                }
                else
                {
                    break; // Sai do loop se o número do painel for válido
                }
            }
            else
            {
                Console.WriteLine("Painel não encontrado. Verifique o número do painel e tente novamente.");
            }
        } while (true);

        // Solicitar obrigatoriamente o número da Canopla
        string numeroCanopla;
        do
        {
            Console.Write("Digite o Número da Canopla: ");
            numeroCanopla = Console.ReadLine();

            // Verificar se a canopla existe no estoque do Firebase
            var catracaComMesmaCanopla = estoqueCatracasFirebase.FirstOrDefault(c => c.Canopla == numeroCanopla);

            if (catracaComMesmaCanopla != null)
            {
                Console.WriteLine($"CANOPLA JÁ UTILIZADA NA CATRACA ({catracaComMesmaCanopla.NumeroCatraca}) - INSIRA OUTRA CANOPLA");
            }
            else
            {
                var canoplaExistente = novoFirebaseClient.Child("estoqueCanoplas").OnceAsync<Canopla>().Result.Select(item => item.Object).FirstOrDefault(c => c.NumeroCanopla.ToString() == numeroCanopla);

                if (canoplaExistente != null)
                {
                    catraca.Canopla = numeroCanopla;
                    break; // Sai do loop se o número da canopla for válido
                }
                else
                {
                    Console.WriteLine("Canopla não encontrada no estoque. Verifique o número da canopla e tente novamente.");
                }
            }
        } while (true);

        // Preencher informações restantes da catraca e adicioná-la ao estoque
        PreencherInformacoesCatraca(catraca, catraca.ModeloPainelCorrespondente, catraca.MarcaPainelCorrespondente);

        catraca.CodigoCatraca = Catraca.GerarCodigoCatraca(catraca);
        estoqueCatracasFirebase.Add(catraca);
        Console.WriteLine("\nCatraca adicionada ao estoque com sucesso!");

        // Salvar catraca no Firebase
        novoFirebaseClient.Child("estoqueCatracas").PostAsync(catraca);
    }




    private static void PreencherInformacoesCatraca(Catraca catraca, ModeloPainel modeloPainel, MarcaPainel marcaPainel)
    {
        var novoFirebaseClient = new FirebaseClient("https://estoque-4d5c2-default-rtdb.firebaseio.com/");
        var estoqueCatracasFirebase = novoFirebaseClient.Child("estoqueCatracas").OnceAsync<Catraca>().Result.Select(item => item.Object).ToList();
        string numeroCatracaInput;
        do
        {
            Console.Write("Digite o Número da Catraca: ");
            numeroCatracaInput = Console.ReadLine();

            // Verificar se a catraca com o mesmo número existe no estoque do Firebase
            var catracaExistente = estoqueCatracasFirebase.FirstOrDefault(c => c.NumeroCatraca == int.Parse(numeroCatracaInput));

            if (catracaExistente == null)
            {
                catraca.NumeroCatraca = int.Parse(numeroCatracaInput);
                break; // Sai do loop, pois a catraca não existe no estoque
            }
            else
            {
                Console.WriteLine("Já existe uma catraca com o mesmo número. Escolha outro número.");
            }
        } while (true);


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

        // Obter a referência da catraca existente no Firebase
        var novoFirebaseClient = new FirebaseClient("https://estoque-4d5c2-default-rtdb.firebaseio.com/");

        var catracaExistente = novoFirebaseClient
            .Child("estoqueCatracas")
            .OrderByKey()
            .OnceAsync<Catraca>()
            .Result
            .Select(snapshot => snapshot.Object)
            .FirstOrDefault(catraca => catraca.NumeroCatraca == numeroCatraca);

        if (catracaExistente != null)
        {
            // Preencher apenas as informações adicionais que faltam
            if (string.IsNullOrWhiteSpace(catracaExistente.NF) || string.IsNullOrWhiteSpace(catracaExistente.Transportadora) || catracaExistente.Saida == null)
            {
                PreencherInformacoesAdicionais(catracaExistente);

                // Atualizar as informações no Firebase
                var catracaFirebaseReference = novoFirebaseClient
                    .Child("estoqueCatracas")
                    .OrderByKey()
                    .OnceAsync<Catraca>()
                    .Result
                    .Where(snapshot => snapshot.Object.NumeroCatraca == numeroCatraca)
                    .Select(snapshot => snapshot.Key)
                    .FirstOrDefault();

                novoFirebaseClient
                    .Child("estoqueCatracas")
                    .Child(catracaFirebaseReference)
                    .PutAsync(catracaExistente);

                Console.WriteLine("\nInformações adicionais preenchidas e salvas com sucesso!");
            }
            else
            {
                Console.WriteLine("\nTodas as informações adicionais já foram preenchidas anteriormente.");
            }
        }
        else
        {
            Console.WriteLine("\nCatraca não encontrada no estoque.");
        }
    }

    private static void PreencherInformacoesAdicionais(Catraca catraca)
    {
        // Solicitar obrigatoriamente a NF, o ID do Cliente e a Transportadora
        do
        {
            if (string.IsNullOrWhiteSpace(catraca.NF))
            {
                Console.Write("Digite a NF: ");
                catraca.NF = Console.ReadLine();
            }

            if (string.IsNullOrWhiteSpace(catraca.IdCliente))
            {
                Console.Write("Digite o ID do Cliente: ");
                catraca.IdCliente = Console.ReadLine();
            }

            if (string.IsNullOrWhiteSpace(catraca.Transportadora))
            {
                Console.Write("Digite a Transportadora: ");
                catraca.Transportadora = Console.ReadLine();
            }

            if (string.IsNullOrWhiteSpace(catraca.NF) || string.IsNullOrWhiteSpace(catraca.IdCliente) || string.IsNullOrWhiteSpace(catraca.Transportadora))
            {
                Console.WriteLine("Todos os campos são obrigatórios. Por favor, insira os dados necessários.");
            }

        } while (string.IsNullOrWhiteSpace(catraca.NF) || string.IsNullOrWhiteSpace(catraca.IdCliente) || string.IsNullOrWhiteSpace(catraca.Transportadora));

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

                throw new InvalidOperationException("Modelo de catraca não reconhecido.");
        }
    }
    private static void BuscarPorCodigoCatraca()
    {
        do
        {
            Console.Write("Digite o código da catraca: ");
            string codigo = Console.ReadLine();

            var catracaEncontrada = BuscarCatracaPorCodigo(codigo);

            if (catracaEncontrada != null)
            {
                Console.WriteLine("\n### Catraca Encontrada ###");
                ExibirDetalhesCatraca(catracaEncontrada);
            }
            else
            {
                Console.WriteLine("\nCatraca não encontrada no estoque.");
            }

            Console.Write("\nDeseja realizar nova consulta? (S/N): ");
            string resposta = Console.ReadLine();

            if (resposta.ToUpper() != "S")
            {
                break;
            }

        } while (true);
    }
    private static Catraca BuscarCatracaPorCodigo(string codigo)
    {
        var novoFirebaseClient = new FirebaseClient("https://estoque-4d5c2-default-rtdb.firebaseio.com/");

        var estoqueCatracasFirebase = novoFirebaseClient.Child("estoqueCatracas").OnceAsync<Catraca>().Result.Select(item => item.Object).ToList();

        var catracaEncontrada = estoqueCatracasFirebase.FirstOrDefault(catraca => catraca.CodigoCatraca == codigo);

        return catracaEncontrada;
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
    private static async void ExibirEstoquePaineis()
    {
        Console.WriteLine("\n### Estoque de Paineis Testados ###");

        // Consulta ao Firebase para obter os painéis
        FirebaseClient firebaseClient = new FirebaseClient("https://estoque-4d5c2-default-rtdb.firebaseio.com/");
        var paineis = await firebaseClient.Child("estoquePaineis").OnceAsync<Painel>();

        if (paineis == null || !paineis.Any())
        {
            Console.WriteLine("Não há painéis testados no estoque.");
            return;
        }

        Console.WriteLine("{0,-15} {1,-15} {2,-15} {3,-15} {4,-25} {5,-15}",
            "Número", "Modelo", "Marca", "Data de Teste", "Código", "Responsável");

        foreach (var painelTestado in paineis)
        {
            var painel = painelTestado.Object;

            Console.WriteLine("{0,-15} {1,-15} {2,-15} {3,-15} {4,-25} {5,-15}",
                painel.NumeroPainel,
                GetModeloPainel(painel.ModeloPainel),
                GetMarcaPainel(painel.Marca),
                painel.DataTeste.ToString("dd/MM/yy"),
                painel.CodigoPainel,
                painel.ResponsavelPainel);
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
                
                throw new InvalidOperationException("Marca de painel não reconhecido.");
        }
    }
    private static async void AdicionarPainelEstoque()
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
            FirebaseClient firebaseClient = new FirebaseClient("https://estoque-4d5c2-default-rtdb.firebaseio.com/");
            await firebaseClient.Child("estoquePaineis").PostAsync(novoPainel);

        }


        Console.WriteLine($"{quantidade} painéis adicionados ao estoque. Último painel adicionado: {estoquePaineis.Last().NumeroPainel}");
    }
    private static void BuscarPorCodigoPainel()
    {
        do
        {
            Console.Write("Digite o código do painel: ");
            string codigoBusca = Console.ReadLine();

            var novoFirebaseClient = new FirebaseClient("https://estoque-4d5c2-default-rtdb.firebaseio.com/");

            var estoquePaineisFirebase = novoFirebaseClient.Child("estoquePaineis").OnceAsync<Painel>().Result.Select(item => item.Object).ToList();

            var painelEncontrado = estoquePaineisFirebase.FirstOrDefault(p => p.CodigoPainel == codigoBusca);

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

            Console.Write("Deseja fazer uma nova busca? (S/N): ");
            string resposta = Console.ReadLine();

            if (resposta?.Trim().ToUpper() != "S")
            {
                break;
            }

        } while (true);
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
    private static async Task ExibirEstoqueCanoplas()
    {
        Console.WriteLine("\n### Estoque de Canoplas ###");

        FirebaseClient firebaseClient = new FirebaseClient("https://estoque-4d5c2-default-rtdb.firebaseio.com/");

        // Buscar canoplas do Firebase
        var response = await firebaseClient.Child("estoqueCanoplas").OnceAsync<Canopla>();

        if (response == null || !response.Any())
        {
            Console.WriteLine("Não há canoplas no estoque.");
            return;
        }

        Console.WriteLine("{0,-15} {1,-15} {2,-15} {3,-25}",
            "Número", "Data de Entrada", "Responsável", "Código");

        foreach (var canopla in response.Select(c => c.Object))
        {
            Console.WriteLine("{0,-15} {1,-15} {2,-15} {3,-25}",
                canopla.NumeroCanopla,
                canopla.DataEntrada.ToString("dd/MM/yy"),
                canopla.ResponsavelCanopla,
                canopla.CodigoCanopla);
        }
    }

    private static async void AdicionarCanoplaEstoque()
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
               
            };

            // Gerar código da canopla
            string codigoCanopla = $"CAN{proximoNumeroCanopla}{responsavelCanopla.Substring(0, Math.Min(2, responsavelCanopla.Length))}";
            novaCanopla.CodigoCanopla = codigoCanopla;

            estoqueCanoplas.Add(novaCanopla);
            FirebaseClient firebaseClient = new FirebaseClient("https://estoque-4d5c2-default-rtdb.firebaseio.com/");
            await firebaseClient.Child("estoqueCanoplas").PostAsync(novaCanopla);

            Console.WriteLine($"Canopla {proximoNumeroCanopla} adicionada ao estoque. Código: {codigoCanopla}");

            proximoNumeroCanopla++;

        }


        Console.WriteLine($"{quantidade} canoplas adicionadas ao estoque. Última canopla adicionada: {estoqueCanoplas.Last().NumeroCanopla}");
    }
    private static void BuscarPorCodigoCanopla()
    {
        do
        {
            Console.Write("Digite o código da canopla: ");
            string codigoBusca = Console.ReadLine();

            var novoFirebaseClient = new FirebaseClient("https://estoque-4d5c2-default-rtdb.firebaseio.com/");

            var estoqueCanoplasFirebase = novoFirebaseClient.Child("estoqueCanoplas").OnceAsync<Canopla>().Result.Select(item => item.Object).ToList();

            var canoplaEncontrada = estoqueCanoplasFirebase.FirstOrDefault(c => c.CodigoCanopla == codigoBusca);

            if (canoplaEncontrada != null)
            {
                Console.WriteLine("\n### Informações da Canopla Encontrada ###");
                Console.WriteLine($"Número da Canopla: {canoplaEncontrada.NumeroCanopla}");
                Console.WriteLine($"Data de Entrada: {canoplaEncontrada.DataEntrada.ToString("dd/MM/yy")}");
                Console.WriteLine($"Responsável: {canoplaEncontrada.ResponsavelCanopla}");
                Console.WriteLine($"Código: {canoplaEncontrada.CodigoCanopla}");
               
            }
            else
            {
                Console.WriteLine("Canopla não encontrada. Verifique o código informado.");
            }

            Console.Write("Deseja fazer uma nova busca? (S/N): ");
            string resposta = Console.ReadLine();

            if (resposta?.Trim().ToUpper() != "S")
            {
                break;
            }

        } while (true);
    }
    //excluir peça do estoque
    public static void MenuExcluirPeca(FirebaseClient firebaseClient)
    {
        do
        {
            Console.WriteLine("\n### Excluir Peça do Estoque ###");
            Console.WriteLine("Escolha o tipo de peça a ser excluída:");
            Console.WriteLine("1. Catraca");
            Console.WriteLine("2. Painel");
            Console.WriteLine("3. Canopla");
            Console.WriteLine("0. Voltar");

            Console.Write("Escolha uma opção: ");
            string tipoPecaInput = Console.ReadLine(); 

            if (tipoPecaInput == "0")
            {
                break;
            }

            if (!int.TryParse(tipoPecaInput, out int tipoPeca) || tipoPeca < 1 || tipoPeca > 3)
            {
                Console.WriteLine("Opção inválida. Tente novamente.");
                Console.ReadKey(); // Aguarda a entrada do usuário antes de continuar
                continue;
            }

            string nomeBanco;
            switch (tipoPeca)
            {
                case 1:
                    nomeBanco = "estoqueCatracas";
                    break;
                case 2:
                    nomeBanco = "estoquePaineis";
                    break;
                case 3:
                    nomeBanco = "estoqueCanoplas";
                    break;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    continue;
            }

            Console.WriteLine($"Conectando ao banco {nomeBanco}...");

            string tipoPecaDescricao = ((ModeloEstoque)tipoPeca - 1 == ModeloEstoque.Catraca)
                ? "Catraca"
                : ((ModeloEstoque)tipoPeca - 1 == ModeloEstoque.Painel)
                    ? "Painel"
                    : "Canopla";

            Console.Write($"Digite o número da {tipoPecaDescricao}: ");

            string numeroBuscaInput = Console.ReadLine();

            if (!int.TryParse(numeroBuscaInput, out int numeroBusca))
            {
                Console.WriteLine("Número de peça inválido. Tente novamente.");
                Console.ReadKey(); // Aguarda a entrada do usuário antes de continuar
                continue;
            }

            switch (tipoPeca)
            {
                case 1:
                    ExcluirPecaDoEstoqueCatraca(firebaseClient, nomeBanco, numeroBusca);
                    break;
                case 2:
                    ExcluirPecaDoEstoquePainel(firebaseClient, nomeBanco, numeroBusca);
                    break;
                case 3:
                    ExcluirPecaDoEstoqueCanopla(firebaseClient, nomeBanco, numeroBusca);
                    break;
            }

        } while (true);
    }
    private static async Task ExcluirPecaDoEstoqueCatraca(FirebaseClient firebaseClient, string nomeBanco, int numeroBusca)
    {
        var estoqueCatracasFirebase = firebaseClient.Child(nomeBanco).OnceAsync<Catraca>().Result;

        var catracaEncontrada = estoqueCatracasFirebase
            .Select(item => item.Object)
            .FirstOrDefault(c => c.NumeroCatraca == numeroBusca);

        if (catracaEncontrada != null)
        {
            Console.WriteLine("\n### Informações da Catraca Encontrada ###");
            Console.WriteLine($"Número da Catraca: {catracaEncontrada.NumeroCatraca}");
            Console.WriteLine($"Código: {catracaEncontrada.CodigoCatraca}");
           

            string catracaKey = estoqueCatracasFirebase
            .Where(item => item.Object.NumeroCatraca == numeroBusca)
            .Select(item => item.Key)
            .FirstOrDefault();

            if (catracaKey != null)
            {
                Console.Write("Deseja excluir esta catraca? (S/N): ");
                string resposta = Console.ReadLine();

                if (resposta?.Trim().ToUpper() == "S")
                {
                    await firebaseClient.Child(nomeBanco).Child(catracaKey).DeleteAsync();
                    Console.WriteLine("\nCatraca excluída do estoque.");
                }
                else
                {
                    Console.WriteLine("\nOperação de exclusão cancelada.");
                }
            }
            else
            {
                Console.WriteLine("Catraca não encontrada. Verifique o número informado.");
            }
        }
        else
        {
            Console.WriteLine("Catraca inexistente. Verifique o número informado.");
        }
    }
    private static async Task ExcluirPecaDoEstoquePainel(FirebaseClient firebaseClient, string nomeBanco, int numeroBusca)
    {
        var estoquePaineisFirebase = firebaseClient.Child(nomeBanco).OnceAsync<Painel>().Result;

        var painelEncontrado = estoquePaineisFirebase
            .Select(item => item.Object)
            .FirstOrDefault(p => p.NumeroPainel == numeroBusca);

        if (painelEncontrado != null)
        {
            Console.WriteLine("\n### Informações do Painel Encontrado ###");
            Console.WriteLine($"Número do Painel Testado: {painelEncontrado.NumeroPainel}");
            Console.WriteLine($"Código do Painel Testado: {painelEncontrado.CodigoPainel}");
           

            string painelKey = estoquePaineisFirebase
            .Where(item => item.Object.NumeroPainel == numeroBusca)
            .Select(item => item.Key)
            .FirstOrDefault();

            if (painelKey != null)
            {
                Console.Write("Deseja excluir este painel? (S/N): ");
                string resposta = Console.ReadLine();

                if (resposta?.Trim().ToUpper() == "S")
                {
                    await firebaseClient.Child(nomeBanco).Child(painelKey).DeleteAsync();
                    Console.WriteLine("\nPainel excluído do estoque.");
                }
                else
                {
                    Console.WriteLine("\nOperação de exclusão cancelada.");
                }
            }
            else
            {
                Console.WriteLine("Painel inexistente. Verifique o número informado.");
            }
        }
        else
        {
            Console.WriteLine("Painel inexistente. Verifique o número informado.");
        }
    }
    private static async Task ExcluirPecaDoEstoqueCanopla(FirebaseClient firebaseClient, string nomeBanco, int numeroBusca)
    {
        var estoqueCanoplasFirebase = firebaseClient.Child(nomeBanco).OnceAsync<Canopla>().Result;

        var canoplaEncontrada = estoqueCanoplasFirebase
            .Select(item => item.Object)
            .FirstOrDefault(c => c.NumeroCanopla == numeroBusca);

        if (canoplaEncontrada != null)
        {
            Console.WriteLine("\n### Informações da Canopla Encontrada ###");
            Console.WriteLine($"Número da Canopla: {canoplaEncontrada.NumeroCanopla}");
            Console.WriteLine($"Código da Canopla: {canoplaEncontrada.CodigoCanopla}");
          

            string canoplaKey = estoqueCanoplasFirebase
            .Where(item => item.Object.NumeroCanopla == numeroBusca)
            .Select(item => item.Key)
            .FirstOrDefault();

            if (canoplaKey != null)
            {
                Console.Write("Deseja excluir esta canopla? (S/N): ");
                string resposta = Console.ReadLine();

                if (resposta?.Trim().ToUpper() == "S")
                {
                    await firebaseClient.Child(nomeBanco).Child(canoplaKey).DeleteAsync();
                    Console.WriteLine("\nCanopla excluída do estoque.");
                }
                else
                {
                    Console.WriteLine("\nOperação de exclusão cancelada.");
                }
            }
            else
            {
                Console.WriteLine("Canopla inexistente. Verifique o número informado.");
            }
        }
        else
        {
            Console.WriteLine("Canopla inexistente. Verifique o número informado.");
        }
    }

    // fim excluir peça do estoque

    // relatórios serão aqui
    //

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


    public enum ModeloEstoque
    {
        Catraca,
        Painel,
        Canopla
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
        public string GetKey()
        {
            return NumeroPainel.ToString(); 
        }
    }

    public class Canopla
    {
        public int NumeroCanopla { get; set; }
        public DateTime DataEntrada { get; set; }
        public string ResponsavelCanopla { get; set; }
        public string CodigoCanopla { get; set; }
        public string GetKey()
        {
            return NumeroCanopla.ToString(); 
        }
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
            string duasLetrasResponsavel = catraca.ResponsavelCatraca.Substring(0, 2).ToUpper(); 
            string numeroPainel = catraca.NumeroPainel;
            string numeroCanopla = catraca.Canopla;

            return $"C{numeroCatraca}{duasLetrasResponsavel}-{numeroPainel}-{numeroCanopla}";
        }
        public string GetKey()
        {
            return NumeroCatraca.ToString();
        }
    }

}











