using System.Net;
using System.Net.Mail;
using System.Text;

namespace IronGym.Application.Services
{
    public class EmailService
    {
        private static readonly Random random = new Random();
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public string SendVerificationEmail(string userEmail)
        {
            try
            {
                string verificationCode = GenerateRandomDigits(6);
                MailMessage mail = WriteVerificationEmail(userEmail, verificationCode);
                SmtpClient smtp = PrepareSending();
                smtp.Send(mail);
                return verificationCode;
            }
            catch
            {
                return "";
            }
        }

        public string SendPasswordEmail(string userEmail)
        {
            try
            {
                string passwordCode = GenerateRandomString(8);
                MailMessage mail = WritePasswordEmail(userEmail, passwordCode);
                SmtpClient smtp = PrepareSending();
                smtp.Send(mail);
                return passwordCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }

        public static MailMessage WriteVerificationEmail(string userEmail, string verificationCode)
        {
            MailMessage message = new MailMessage();
            message.To.Add(userEmail);
            message.From = new MailAddress("soportepotengym@gmail.com");

            message.Subject = "Código de Verificación - POTEN GYM";
            message.IsBodyHtml = true;
            message.Body = $@"
            <html>
            <head>
                <style>
                    .email-container {{
                        font-family: Arial, sans-serif;
                        line-height: 1.6;
                        color: #ffffff;
                        background-color: #000000;
                        padding: 20px;
                        max-width: 600px;
                        margin: auto;
                    }}
                    .email-header h2 {{
                        color: #ff0000;
                        margin: 0;
                    }}
                    .verification-code {{
                        font-size: 1.5em;
                        color: #ff0000;
                        font-weight: bold;
                        margin: 20px 0;
                    }}
                    .email-footer {{
                        font-size: 0.9em;
                        color: #cccccc;
                        text-align: center;
                        margin-top: 20px;
                    }}
                </style>
            </head>
            <body>
                <div class='email-container'>
                    <div class='email-header'>
                        <h2>POTEN GYM - Verificación</h2>
                    </div>
                    <p>Hola,</p>
                    <p>Gracias por registrarte en <strong>POTEN GYM</strong>. Para completar el proceso de verificación, introduce el siguiente código:</p>
                    <p class='verification-code'>{verificationCode}</p>
                    <p>Si no solicitaste este código, ignora este mensaje o contacta con soporte.</p>
                    <div class='email-footer'>
                        &copy; 2024 POTEN GYM. Todos los derechos reservados.
                    </div>
                </div>
            </body>
            </html>";
            return message;
        }

        public static MailMessage WritePasswordEmail(string userEmail, string password)
        {
            MailMessage message = new MailMessage();
            message.To.Add(userEmail);
            message.From = new MailAddress("soportepotengym@gmail.com");

            message.Subject = "Tu contraseña de acceso - POTEN GYM";
            message.IsBodyHtml = true;
            message.Body = $@"
            <html>
            <head>
                <style>
                    .email-container {{
                        font-family: Arial, sans-serif;
                        line-height: 1.6;
                        color: #ffffff;
                        background-color: #000000;
                        padding: 20px;
                        max-width: 600px;
                        margin: auto;
                    }}
                    .email-header h2 {{
                        color: #ff0000;
                        margin: 0;
                    }}
                    .password {{
                        font-size: 1.5em;
                        color: #ff0000;
                        font-weight: bold;
                        margin: 20px 0;
                    }}
                    .email-footer {{
                        font-size: 0.9em;
                        color: #cccccc;
                        text-align: center;
                        margin-top: 20px;
                    }}
                </style>
            </head>
            <body>
                <div class='email-container'>
                    <div class='email-header'>
                        <h2>Bienvenido a POTEN GYM</h2>
                    </div>
                    <p>Hola,</p>
                    <p>¡Bienvenido al equipo de <strong>POTEN GYM</strong>! A continuación, encontrarás tu contraseña de acceso:</p>
                    <p class='password'>{password}</p>
                    <p>Utiliza esta contraseña junto con tu correo electrónico para iniciar sesión.</p>
                    <p>Si no esperabas este mensaje, por favor contacta con soporte inmediatamente.</p>
                    <div class='email-footer'>
                        &copy; 2024 POTEN GYM. Todos los derechos reservados.
                    </div>
                </div>
            </body>
            </html>";
            return message;
        }

        private static SmtpClient PrepareSending()
        {
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.EnableSsl = true;
            smtp.Port = 587;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential("soportepotengym@gmail.com", "zohh frcm oqhx uwjn");
            return smtp;
        }

        public static string GenerateRandomDigits(int numberOfDigits)
        {
            string verificationCode = "";
            Random random = new Random();
            for (int i = 0; i < numberOfDigits; i++)
            {
                int randomNumber = random.Next(10);
                verificationCode += randomNumber.ToString();
            }
            return verificationCode;
        }

        public string GenerateRandomString(int length)
        {
            if (length < 1)
                throw new ArgumentException("La longitud debe ser mayor que cero", nameof(length));

            var stringBuilder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(chars[random.Next(chars.Length)]);
            }
            return stringBuilder.ToString();
        }
    }
}
