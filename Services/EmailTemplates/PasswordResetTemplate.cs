using System;
using PropertyBase.Entities;

namespace PropertyBase.Services.EmailTemplates
{
    public static class PasswordResetTemplate
    {
      public static string GenerateTemplate(User user, string frontendUrlPath)
        {
            return $@"
              <!DOCTYPE html>
                <html lang=""en"">
                  <head>
                    <meta charset=""UTF-8"" />
                    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                    <title>Password Reset</title>
                    <style>
                      .btn {{
                        padding: 0.8rem;
                        border-radius: 0.5rem;
                        outline: none;
                        border: none;
                        font-weight: 500;
                        color: white;
                        background-color: #0b0b9f;
                        cursor: pointer;
                        font-size: 16px;
                        letter-spacing: 2px;
                        text-decoration: none;
                      }}
                      .btn:hover {{
                        background-color: #080871;
                      }}
                      .link {{
                        color: #0b0be3;
                        font-weight: bold;
                        text-decoration: none;
                      }}
                      .link:hover {{
                        text-decoration: underline;
                      }}
                    </style>
                    <script>
                       function openUrl(url) {{
                         window.open(url);
                         }}
                    </script>
                  </head>
                  <body>
                    <div style=""margin-right: auto; margin-left: auto"">
                      <p>Hi <b>{user.FirstName}</b></p>

                      <p>
                        You recently requested to reset your password with this email.
                      </p>
                      <p>
                       If this request was made by you, please click on this <a class=""link"" href=""{frontendUrlPath}"">link</a><br/>
                       to reset your password
                      </p>
                      

                      <p style=""color: #494747; margin-top: 2rem; font-style: italic"">
                        <b>- Propery Forager Team -</b>
                      </p>
                    </div>
                  </body>
                </html>

            ";
        }
    }
}

