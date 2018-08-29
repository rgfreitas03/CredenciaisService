using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace SingleSignOn.Configurations
{
    public class SigningConfiguration
    {

        public SecurityKey Key { get; }
        public SigningCredentials SigningCredentials { get; }
        public IConfiguration Configuration { get; }

        public SigningConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;
            using (var provider = new RSACryptoServiceProvider(2048))
            {
                Key = new RsaSecurityKey(provider.ExportParameters(true));
            }

            SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.RsaSha256Signature);
        }

        public SymmetricSecurityKey GenerateKey()
            => new SymmetricSecurityKey(this.Certificate().Certificate.GetRawCertData());
        //=> new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("Signature").GetSection("PrivateKey").Value));

        public SigningCredentials Credentials()
            => new SigningCredentials(this.GenerateKey(), SecurityAlgorithms.HmacSha256);

        public X509SecurityKey Certificate()
        {
            var signatureConfig = Configuration.GetSection("Signature");
            var cert = new X509Certificate2(signatureConfig.GetSection("File").Value, 
                signatureConfig.GetSection("Password").Value);
            return new X509SecurityKey(cert);
        }


    }
}