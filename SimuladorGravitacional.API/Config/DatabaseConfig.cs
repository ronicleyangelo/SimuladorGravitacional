// using Microsoft.Data.SqlClient;

// namespace ProgramacaoAvancada.Configuration
// {
//     /// <summary>
//     /// Classe de configuração centralizada para conexão com SQL Server
//     /// </summary>
//     public class DatabaseConfig
//     {
//         // ========== CONFIGURAÇÕES DO SERVIDOR ==========
//         public string Server { get; set; } = "localhost";
//         public string Port { get; set; } = "1433";

//         // ========== CONFIGURAÇÕES DO BANCO ==========
//         public string Database { get; set; } = "master";

//         // ========== TIPO DE AUTENTICAÇÃO ==========
//         public bool UseWindowsAuthentication { get; set; } = false;  // ← Mudado para false (você usa sa)

//         // ========== CREDENCIAIS (apenas se UseWindowsAuthentication = false) ==========
//         public string UserId { get; set; } = "sa";
//         public string Password { get; set; } = "1234";

//         // ========== CONFIGURAÇÕES AVANÇADAS ==========
//         public int ConnectionTimeout { get; set; } = 30;
//         public bool MultipleActiveResultSets { get; set; } = true;
//         public bool TrustServerCertificate { get; set; } = true;
//         public bool Encrypt { get; set; } = false;

//         /// <summary>
//         /// Gera a Connection String completa baseada nas configurações
//         /// </summary>
//         public string GetConnectionString()
//         {
//             var builder = new SqlConnectionStringBuilder
//             {
//                 DataSource = string.IsNullOrEmpty(Port) ? Server : $"{Server},{Port}",
//                 InitialCatalog = Database,
//                 ConnectTimeout = ConnectionTimeout,
//                 MultipleActiveResultSets = MultipleActiveResultSets,
//                 TrustServerCertificate = TrustServerCertificate,
//                 Encrypt = Encrypt
//             };

//             if (UseWindowsAuthentication)
//             {
//                 builder.IntegratedSecurity = true;
//             }
//             else
//             {
//                 builder.UserID = UserId;
//                 builder.Password = Password;
//             }

//             return builder.ConnectionString;
//         }

//         /// <summary>
//         /// Configuração padrão para desenvolvimento local (SUA CONFIGURAÇÃO)
//         /// </summary>
//         public static DatabaseConfig Local => new DatabaseConfig
//         {
//             Server = "localhost",
//             Port = "1433",
//             Database = "master",
//             UseWindowsAuthentication = false,  // ← SQL Server Auth
//             UserId = "sa",
//             Password = "1234",
//             TrustServerCertificate = true
//         };

//         /// <summary>
//         /// Configuração para produção (ajustar conforme necessário)
//         /// </summary>
//         public static DatabaseConfig Production => new DatabaseConfig
//         {
//             Server = "seu-servidor-producao",
//             Port = "1433",
//             Database = "ProgramacaoAvancadaDB",
//             UseWindowsAuthentication = false,
//             UserId = "seu_usuario_producao",
//             Password = "sua_senha_segura_producao",
//             TrustServerCertificate = false,
//             Encrypt = true
//         };

//         /// <summary>
//         /// Retorna informações de conexão (sem exibir senha)
//         /// </summary>
//         public string GetConnectionInfo()
//         {
//             return $"Servidor: {Server}:{Port} | Banco: {Database} | " +
//                    $"Autenticação: {(UseWindowsAuthentication ? "Windows" : "SQL Server (" + UserId + ")")}";
//         }
//     }
// }