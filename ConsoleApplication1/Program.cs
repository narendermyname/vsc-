using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultSharp.Backends.Authentication.Models;
using VaultSharp.Backends.Authentication.Models.GitHub;

namespace HashiVaultClient
{
    class InMemoryHashiVaultClient
    {
        private Dictionary<String, Object> hashiVault = new Dictionary<String, Object>();

        private VaultSharp.IVaultClient vaultClient;
        private String vaultToken;
        private String vaultURL;
        private Boolean cashedPassword;

        private static readonly String VAULT_TOKEN_FILE_NAME = ".vault-token";
        private static readonly String WINDOW_OD_NAME = "Windows";
        private static readonly String ENV = "env";

        public InMemoryHashiVaultClient(Boolean cashedPass)
        {
            this.cashedPassword = cashedPass;
            if (this.vaultURL == null)
            {
                this.vaultURL = getVaultURL();
            }
            Console.WriteLine("Vault URL " + vaultURL);
            initializeToken();
            vaultClient = VaultSharp.VaultClientFactory.CreateVaultClient(new Uri(vaultURL),
                new VaultSharp.Backends.Authentication.Models.Token.TokenAuthenticationInfo(vaultToken));
        }
        static void Main(string[] args)
        {
            InMemoryHashiVaultClient client = new InMemoryHashiVaultClient(true);
            //client.getPassword("user");

            //IAuthenticationInfo gitHubAuthenticationInfo = new GitHubAuthenticationInfo("user", "000000");
            //VaultSharp.IVaultClient vaultClient = VaultSharp.VaultClientFactory.CreateVaultClient(new Uri("https://api.github.com"), gitHubAuthenticationInfo);
            //String value = (String)vaultClient.ReadSecretAsync("user").Result.Data["user"];
            Console.ReadKey();
        }

        private void initializeToken()
        {
            vaultToken = readVaultTokenFromFile(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) +
                Path.DirectorySeparatorChar + VAULT_TOKEN_FILE_NAME);
            Console.WriteLine("Vault Token " + vaultToken);
        }

        /*
         * Get password from WMCHashiVault using hashiPath 
         */
        private String getPassword(String hashiPath)
        {
            String secret = null;
            try
            {
                vaultClient.WriteSecretAsync(hashiPath, hashiVault);

                secret = (String)vaultClient.ReadSecretAsync(hashiPath).Result.Data[hashiPath];
            }
            catch (Exception ex)
            {
                throw new HashiVaultException("Exception thrown while retrieving the password for the key :" + hashiPath, ex);
            }

            return secret;
        }

        private String getVaultURL()
        {
            String env = "test";
            return "https://hv-" + env + ".wellmanage.com";
        }

        private String readVaultTokenFromFile(String filePath)
        {
            // Reading vaultTokenFile
            String vaultToken = null;
            System.IO.StreamReader vaultTokenFile = null;
            try
            {
                vaultTokenFile = new System.IO.StreamReader(filePath);
                vaultToken = vaultTokenFile.ReadLine();

            }
            catch (Exception ex)
            {
                throw new HashiVaultException("Token reading from " + filePath + " path has failed.", ex);
            }
            finally
            {
                vaultTokenFile.Close();
            }
            return vaultToken;
        }
    }

    class HashiVaultException : System.Exception
    {
        public HashiVaultException() : base()
        {
        }

        public HashiVaultException(String message) : base(message)
        {

        }

        public HashiVaultException(String message, Exception innerException) : base(message, innerException)

        {

        }
    }


}