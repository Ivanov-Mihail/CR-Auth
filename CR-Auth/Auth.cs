using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace CR_Auth
{
	public class Auth
	{
		public Auth(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public List<PayloadRows> PayloadRows = new List<PayloadRows>();

		public async Task<AuthUser> GetPayloadAfterValidation(string bearerToken)
		{
			var token = bearerToken.Replace("Bearer ", "");
			var jsonWebToken = new JsonWebToken(token);


			// 1st implementation
			AuthUser authUserData;
			{
				string id = null;
				string[] roles = null;
				string[] permissions = null;

				jsonWebToken.TryGetPayloadValue<string>("id", out id);
				jsonWebToken.TryGetPayloadValue<string[]>("roles", out roles);
				jsonWebToken.TryGetPayloadValue<string[]>("permissions", out permissions);
				authUserData = new AuthUser(id, roles, permissions);

			}


			var securityTokenHandler = new JwtSecurityTokenHandler();
			var publicKey = _configuration["Jwt:Key"];
			var options = GetTokenValidationParameters(publicKey);

			var claimPrincipal = await Task.Run(() =>
				securityTokenHandler.ValidateToken(bearerToken, options, out var securityToken));


			if (null != claimPrincipal)
			{
				foreach (var claim in claimPrincipal.Claims)
				{
					var row = new PayloadRows
					{
						Key = claim.Type,
						Value = claim.Value
					};
					PayloadRows.Add(row);
				}
			}

			return authUserData;

		}

		private readonly IConfiguration _configuration;
		private TokenValidationParameters GetTokenValidationParameters(string key)
		{

			var rs256Token = key.Replace("-----\nBEGIN PUBLIC KEY-----", "");
			rs256Token = rs256Token.Replace("\n-----END PUBLIC KEY-----", "");

			var keyBytes = Convert.FromBase64String(rs256Token);
			var asymmetricKeyParameter = PublicKeyFactory.CreateKey(keyBytes);
			var rsaKeyParameters = (RsaKeyParameters)asymmetricKeyParameter;
			var rsaParameters = new RSAParameters
			{
				Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned(),
				Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned()
			};
			var rsa = new RSACryptoServiceProvider();

			rsa.ImportParameters(rsaParameters);

			var validationParameters = new TokenValidationParameters()
			{
				RequireExpirationTime = false,
				RequireSignedTokens = true,
				ValidateAudience = false,
				ValidateIssuer = false,
				IssuerSigningKey = new RsaSecurityKey(rsa),
			};

			return validationParameters;
		}
	}
}
