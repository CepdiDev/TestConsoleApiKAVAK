using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApiKAVAK
{
    class Program
    {
        static void Main(string[] args)
        {
            string Usuario = "kavak@mail.com";
            string Password = "w6gMht4Npw";
			string Lineas = ObtenerLineas();


            Console.WriteLine("--------- Datos Originales ---------");
			Console.WriteLine("Usuario: " + Usuario);
			Console.WriteLine("Password: " + Password);

			Console.WriteLine("\n\n--------- Datos Encriptados ---------");
			Usuario = AESEncrypt(Usuario);
			Password = AESEncrypt(Password);
			Lineas = AESEncrypt(Lineas);
			Console.WriteLine("Usuario: " + Usuario);
			Console.WriteLine("Password: " + Password);


			Console.WriteLine("\n\n--------- Consumo API ---------");
			dynamic clase = new ExpandoObject();
			clase.Usuario =Usuario;
			clase.Password = Password;
			clase.Lineas = Lineas;

			var body = JsonConvert.SerializeObject(clase);

			var client = new RestClient("https://testing.cepdi.mx/ServiceWS/api/Timbrado/TimbrarComprobanteKAVAK");
			client.Timeout = -1;
			var request = new RestRequest(Method.POST);
			request.AddHeader("Content-Type", "application/json");
			request.AddParameter("application/json", body, ParameterType.RequestBody);
			IRestResponse response = client.Execute(request);
			Console.WriteLine(response.StatusCode);
			Console.WriteLine(response.Content);


			Console.WriteLine("\n\n--------- Datos Desencriptados ---------");
			Usuario = AESDecrypt(Usuario);
			Password = AESDecrypt(Password);
			Console.WriteLine("Usuario: " + Usuario);
			Console.WriteLine("Password: " + Password);

			Console.ReadKey(true);
        }

		public static string AESEncrypt(string base64Encode)
		{
			string _KeyEcrypt = "7B650B98F2DB310AD5B3CB3752C51C9D";
			try
			{
				using (var aes = new AesCryptoServiceProvider())
				{
					aes.Key = Encoding.UTF8.GetBytes(_KeyEcrypt);
					aes.Mode = CipherMode.ECB;
					aes.Padding = PaddingMode.PKCS7;
					byte[] encrypted = AESCrypto(CryptoOperation.ENCRYPT, aes, Encoding.UTF8.GetBytes(base64Encode));
					return Convert.ToBase64String(encrypted);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error en la encriptación: " + ex.Message);
			}
		}

		public static string AESDecrypt(string base64Encode)
		{
			string _KeyEcrypt = "7B650B98F2DB310AD5B3CB3752C51C9D";
			try
			{
				using (var aes = new AesCryptoServiceProvider())
				{
					aes.Key = Encoding.UTF8.GetBytes(_KeyEcrypt);
					aes.Mode = CipherMode.ECB;
					aes.Padding = PaddingMode.PKCS7;
					byte[] decrypted = AESCrypto(CryptoOperation.DECRYPT, aes, Convert.FromBase64String(base64Encode));
					return Encoding.UTF8.GetString(decrypted);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error en la desencriptación: " + ex.Message);
			}
		}

		public static byte[] AESCrypto(CryptoOperation cryptoOperation, AesCryptoServiceProvider aes, byte[] message)
		{
			using (var memStream = new MemoryStream())
			{
				CryptoStream cryptoStream = null;

				if (cryptoOperation == CryptoOperation.ENCRYPT)
					cryptoStream = new CryptoStream(memStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
				else if (cryptoOperation == CryptoOperation.DECRYPT)
					cryptoStream = new CryptoStream(memStream, aes.CreateDecryptor(), CryptoStreamMode.Write);

				if (cryptoStream == null)
					return null;

				cryptoStream.Write(message, 0, message.Length);
				cryptoStream.FlushFinalBlock();
				return memStream.ToArray();
			}
		}
		public enum CryptoOperation
		{
			ENCRYPT,
			DECRYPT
		};
		public static string ObtenerLineas()
		{
			string Lineas = "" +
						"[CFD]" +
						"FOLIO; 201" +
						"SERIE; AAB" +
						"FECHA; 2021 - 07 - 06T17: 02:35" +
						"FORMADEPAGO; 99" +
						"CONDICIONESDEPAGO; " +
						"SUBTOTAL; 10.00" +
						"DESCUENTO; " +
						"MONEDA; MXN" +
						"TIPODECAMBIO; " +
						"TOTAL; 11.00" +
						"TIPODECOMPROBANTE; I" +
						"METODODEPAGO; PPD" +
						"LUGAREXPEDICION; 03800" +

						"[CFDIRELACIONADOS]" +
						"TIPORELACION; " +
						"TIPORELACIONUUID; " +

						"[DATOS_ADICIONALES_CFDI]" +
						"Adicional1; Soriana" +
						"Adicional2; Chedraui" +
						"Adicional3; HEB" +

						"[EMISOR]" +
						"RFC; XIA190128J61" +
						"NOMBRE; DemoSA DE CV" +
						"REGIMENFISCAL; 601" +

						"[RECEPTOR]" +
						"RFC; XIA190128J61" +
						 "RAZONSOCIAL; Cesar Gabriel gaonaperez" +
						"RESIDENCIAFISCAL; MEX" +
						"NUMREGIDTRIB; " +
						"USOCFDI; P01" +

						"[CONCEPTO]" +
						"CLAVEPRODSERV; 82111500" +
						"NOIDENTIFICACION; SE10019" +
						 "CANTIDAD; 1.00" +
						"CLAVEUNIDAD; SE" +
						"UNIDAD; Servicio" +
						"DESCRIPCION; RENTA DE COMPUTADORAS" +
						"PRECIO; 10.00" +
						"IMPORTE; 10.00" +
						"DESCUENTO; 0.00" +

						"[CONCEPTO_IMPUESTO_TRASLADADO]" +
						"BASE; 10.00" +
						"CODIGO; 002" +
						"TIPOFACTOR; Tasa" +
						"TASAOCUOTA; 0.160000" +
						"IMPORTE; 1.60" +

						"[CONCEPTO_IMPUESTO_RETENIDO]" +
						"BASE; 10.00" +
						"CODIGO; 002" +
						"TIPOFACTOR; Tasa" +
						"TASAOCUOTA; 0.060000" +
						"IMPORTE; 0.60" +

						"[CONCEPTO_DATOS_ADICIONALESI]" +
						"DATOADICIONALCONCEPTO1; prueba1" +
						"DATOADICIONALCONCEPTO2; prueba2" +

						"[IMPUESTO_TOTAL_TRASLADADOS]" +
						"TOTAL; 1.60" +
						"CODIGO; 002" +
						"TIPOFACTOR; Tasa" +
						"TASAOCUOTA; 0.160000" +
						"IMPORTE; 1.60" +

						"[IMPUESTO_TOTAL_RETENIDOS]" +
						"TOTAL; 0.60" +
						"CODIGO; 002" +
						"IMPORTE; 0.60";

			return Lineas;
		}
    }
}
