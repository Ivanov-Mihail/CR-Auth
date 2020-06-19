using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;

namespace CRAP
{

	public class test
	{
		public string Key { get; set; }
		public string Value { get; set; }


		public test()
		{

		}
	}

	public class AuthCityRideModule
	{

		private string _defaultPublickKey = "-----BEGIN PUBLIC KEY-----MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAskm3BMjUXUtCLQu3+XxV7rns9EA56VjXYfozk4kZS6JxGNFVEDacO87F5GHxNguif1LbdtKGoPhUjy4GfSdcNO8c+rPjewvLJfrHnOiQAxsA3gbg0sOXZ6xwuxhfLLVMgj7ZR8Q8TZ0NRLQRhivoxg2NWw94lmAsX8BuchxyznqgR80/sxeTg3v9S6FN6uPW1AGyt8H7IacLt+j2aHYxvBq1iJ6vAcxBqCO1Cu+ztJ1YyeN/8kkWwywoEFSBnuhkLH2Mr0j6ogQ6Nyej+vpfMc4bDWif/xNMOwaeL9cOXmYyr98XRX5YtbIkcuW68k0Eiz0Af/J1JuezZkwRyr+tWbZUwrutfSV220FBOIzGNlvBsRBJH8mav0S02+kJaj7jINX2q8drAGvisJd5UMXJbOQqc0MAnUIROfPqRxOjS7iBImKdunL4d95iKrXFdF2ap7IDcvqYN8Q2WGTXXriXveI9xOpnjgjJQ9FXeunU5JkCvIN00/VmX5OrtKkMhbbHkg17DaWXzv4G8FFKu1WD8F6zIRzlSXML8fBeOyci5CMByiJIf955oamQq998NU6xTtPKDq4Qpa8CuBvMYERoc+ghC33tMhofu5tGqhrAeHEruFgWHZKqbSkna1CsRMokWgh/8rC146tB2pI/CSp2Fod0fP9BuyIqAzs/vIyLUoUCAwEAAQ==-----END PUBLIC KEY-----";
		private string _defaultPublickKey1 = "-----\nBEGIN PUBLIC KEY-----MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAskm3BMjUXUtCLQu3+XxV7rns9EA56VjXYfozk4kZS6JxGNFVEDacO87F5GHxNguif1LbdtKGoPhUjy4GfSdcNO8c+rPjewvLJfrHnOiQAxsA3gbg0sOXZ6xwuxhfLLVMgj7ZR8Q8TZ0NRLQRhivoxg2NWw94lmAsX8BuchxyznqgR80/sxeTg3v9S6FN6uPW1AGyt8H7IacLt+j2aHYxvBq1iJ6vAcxBqCO1Cu+ztJ1YyeN/8kkWwywoEFSBnuhkLH2Mr0j6ogQ6Nyej+vpfMc4bDWif/xNMOwaeL9cOXmYyr98XRX5YtbIkcuW68k0Eiz0Af/J1JuezZkwRyr+tWbZUwrutfSV220FBOIzGNlvBsRBJH8mav0S02+kJaj7jINX2q8drAGvisJd5UMXJbOQqc0MAnUIROfPqRxOjS7iBImKdunL4d95iKrXFdF2ap7IDcvqYN8Q2WGTXXriXveI9xOpnjgjJQ9FXeunU5JkCvIN00/VmX5OrtKkMhbbHkg17DaWXzv4G8FFKu1WD8F6zIRzlSXML8fBeOyci5CMByiJIf955oamQq998NU6xTtPKDq4Qpa8CuBvMYERoc+ghC33tMhofu5tGqhrAeHEruFgWHZKqbSkna1CsRMokWgh/8rC146tB2pI/CSp2Fod0fP9BuyIqAzs/vIyLUoUCAwEAAQ==\n-----END PUBLIC KEY-----";

		private HttpClient _httpCleClient;
		private string _url = "http://" + "stage-app-cityride.eu-central-1.elasticbeanstalk.com/api/v1/me";
		private string _testJwtToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImp0aSI6IjNkOWVkNDI0YTkxOTlmYWJlMzkyNDc2NjQ4ZjU4MTk1NjFkOWQ2NGExYTNkOTA4MDk2M2IxZTgyY2JkNTQzOTIzMTEwYjFmOTcwY2FhZTQwIn0.eyJhdWQiOiIxIiwianRpIjoiM2Q5ZWQ0MjRhOTE5OWZhYmUzOTI0NzY2NDhmNTgxOTU2MWQ5ZDY0YTFhM2Q5MDgwOTYzYjFlODJjYmQ1NDM5MjMxMTBiMWY5NzBjYWFlNDAiLCJpYXQiOjE1OTE3MDI3MzYsIm5iZiI6MTU5MTcwMjczNiwiZXhwIjoxNjIzMjM4NzM2LCJzdWIiOiIyMSIsInNjb3BlcyI6W10sImlkIjoyMSwicm9sZXMiOlsic3VwZXItYWRtaW4iLCJhZG1pbiIsImNsaWVudCJdLCJwZXJtaXNzaW9ucyI6W119.LG8oMjkW_zi1vvcnltGWjSbLFJnST3auESyVdmSHezHNKa93sKKvOTkiVscC5QQSDNyBds-jkQwyUdfkyQkQctU3dXFZV_Ji2nYa42voWsDl-JLabsU-G9Gd9PzDr96Z1gN6zaOZiSDpekdDtn9z_KvFcISjfr6SAzfPkvCStpc0-YrAys1O2HfNsJqomgDIrzbTuAjVfJ7bThzdVE-ilNtK6jFbmoNKsZAHnhLKxHR4txnI-SOmOjgsmzhjblYg2Esm1c1c_z0D1LyisnVGIz15s1kPprI329tjlfOIrYm4PZYnMeXAeHSxVgjrIsCd5VCLLxyvYs6Hfcbp9VFCc-2z9KHpiKnlq0tDruEfqMp5Bp7lzMG_XXMKVotL0pryYISbYqQUVX-65X8rdTdM7qo_LUPYJQfwOhr6951bMEeCrVyfvNvizV26g2KRHghiziFH0gnANF8lrUIOFZMzG3BhOEPw08gEv6ZgdmtqBa68YbhB37abEpEEdUgbTUa7_CLhLs4aAg20h3t563n9f0dlUpEgtwgKxtMB7VZ-suBeV819Eo814he9D3iEr-Z99kt49vdiZUUIikwv12VLyPhjzyziTbHUPdtGPIpxYrH9HfL4upF2M9SqId9m_NsWanXK6zRSlXcYt3n4oFeBB-Xn5ggfY3nqjiBFP37xXUk";


		public AuthCityRideModule()
		{
			_httpCleClient = new HttpClient();
		}

		public AuthCityRideModule(string redisIp, string redisPort, string publicKey)
		{
			RedisIp = redisIp;
			RedisPort = redisPort;
			_httpCleClient = new HttpClient();

		}

		public string RedisIp { get; set; } = "10.0.0.78";
		public string RedisPort { get; set; } = "6379";

		public List<test> Listsss = new List<test>();




		private void GetJson()
		{
			var myJsonString = File.ReadAllText("settings.json");
			
			
		}

		public void JwtTokVerif()
		{
			try
			{

				JsonWebToken jsonWebToken = new JsonWebToken(_testJwtToken);
				var roles = jsonWebToken.GetPayloadValue<string[]>("roles");
				var permissions = jsonWebToken.GetPayloadValue<string[]>("permissions");


				SecurityTokenHandler securityTokenHandler = new JwtSecurityTokenHandler();

				//SecurityToken securityToken = new JsonWebToken(_testJwtToken);

				// SecurityToken securityToken = null; // 100% работает

				var options = GetTokenValidationParameters(_defaultPublickKey1);
				var something = securityTokenHandler.ValidateToken(_testJwtToken, options, out var securityToken);




				if (null != something)
				{
					foreach (Claim claim in something.Claims)
					{
						var test1 = new test();
						test1.Key = claim.Type;
						test1.Value = claim.Value;
						Listsss.Add(test1);
					}
				}


				string[] array = null;
				var anw = jsonWebToken.TryGetPayloadValue<string[]>("roles", out array);
				var anw1 = jsonWebToken.TryGetPayloadValue<string[]>("permissions", out array);

			}
			catch (ArgumentException exception)
			{
				throw exception;
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		private TokenValidationParameters GetTokenValidationParameters(string key)
		{

			var rs256Token = key.Replace("-----BEGIN PUBLIC KEY-----", "");
			rs256Token = rs256Token.Replace("-----END PUBLIC KEY-----", "");
			rs256Token = rs256Token.Replace("\n", "");

			var keyBytes = Convert.FromBase64String("MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAskm3BMjUXUtCLQu3+XxV7rns9EA56VjXYfozk4kZS6JxGNFVEDacO87F5GHxNguif1LbdtKGoPhUjy4GfSdcNO8c+rPjewvLJfrHnOiQAxsA3gbg0sOXZ6xwuxhfLLVMgj7ZR8Q8TZ0NRLQRhivoxg2NWw94lmAsX8BuchxyznqgR80/sxeTg3v9S6FN6uPW1AGyt8H7IacLt+j2aHYxvBq1iJ6vAcxBqCO1Cu+ztJ1YyeN/8kkWwywoEFSBnuhkLH2Mr0j6ogQ6Nyej+vpfMc4bDWif/xNMOwaeL9cOXmYyr98XRX5YtbIkcuW68k0Eiz0Af/J1JuezZkwRyr+tWbZUwrutfSV220FBOIzGNlvBsRBJH8mav0S02+kJaj7jINX2q8drAGvisJd5UMXJbOQqc0MAnUIROfPqRxOjS7iBImKdunL4d95iKrXFdF2ap7IDcvqYN8Q2WGTXXriXveI9xOpnjgjJQ9FXeunU5JkCvIN00/VmX5OrtKkMhbbHkg17DaWXzv4G8FFKu1WD8F6zIRzlSXML8fBeOyci5CMByiJIf955oamQq998NU6xTtPKDq4Qpa8CuBvMYERoc+ghC33tMhofu5tGqhrAeHEruFgWHZKqbSkna1CsRMokWgh/8rC146tB2pI/CSp2Fod0fP9BuyIqAzs/vIyLUoUCAwEAAQ==");

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


		public async void Authenticate()
		{
			try
			{
				var uri = new Uri(_url);
				_httpCleClient.DefaultRequestHeaders.Authorization =
					new AuthenticationHeaderValue("Bearer", _testJwtToken);
				var response = await _httpCleClient.GetAsync(uri);
				var contentString = response.Content.ReadAsStringAsync();
				var res = contentString.Result;
				//var response =  _httpCleClient.GetAsync(uri).Result.Content.ReadAsStringAsync().Result;
			}
			catch
			{
				throw new Exception();
			}
		}


	}
}
