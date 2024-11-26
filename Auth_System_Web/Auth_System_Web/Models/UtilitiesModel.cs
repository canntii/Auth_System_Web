using Auth_System_Web.Services;
using System.Text.RegularExpressions;

namespace Auth_System_Web.Models
{
    public class UtilitiesModel : IUtilitiesModel 
    {

        public async Task<string> convertBase64(string value)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    byte[] imageBytes = await client.GetByteArrayAsync(value);
                    string base64string = Convert.ToBase64String(imageBytes);

                    if (IsBase64String(base64string))
                    {
                        return base64string; // La cadena Base64 es válida
                    }
                    else
                    {
                        return null;
                    }

                }
            } catch (Exception ex) 
            {
                return "Ha surgido un error" + ex;
            }
        }

        private bool IsBase64String(string base64string)
        {
            if(base64string.Length %4 != 0)
            {
                return false;
            }

            // Expresión regular para comprobar si la cadena solo contiene caracteres válidos para Base64
            Regex regex = new Regex(@"^[a-zA-Z0-9\+/]*={0,2}$");
            return regex.IsMatch(base64string);
        }
        
    }
}
