using System;
using PropertyBase.Entities;

namespace PropertyBase.Services.EmailTemplates
{
    public static class PropertyInspectionRequestEmail
    {
        public static string GenerateTemplate(
            string recipientName,
            string requestSenderName,
            string requestSenderEmail,
            string propertyUrl
            )
        {
            return $@"
              <!DOCTYPE html>
                <html lang=""en"">
                  <head>
                    <meta charset=""UTF-8"" />
                    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                    <title>Property Inspection Request</title>
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
                        font-size: 1.1rem;
                        
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
                      <p>Dear <b>{recipientName},</b></p>

                      <p>I hope this email finds you well. I wanted to inform you that <em>{requestSenderName}</em>, whose email address is {requestSenderEmail},
                          has expressed a keen interest in inspecting one of your properties.
                      </p>

                       <p>You can find more details and images of this property <a class=""link"" href=""{propertyUrl}"">here</a>
                      </p>

                      <p>
                        Please let us know the available inspection dates and times, and we will coordinate with {requestSenderName} accordingly.
                      </p>

                      <p>
                        Thank you for your prompt attention to this request. We look forward to facilitating a smooth property inspection process.
                      </p>

                      <p>Warm regards,</p>

                      <p style=""color: #494747; margin-top: 2rem; font-style: italic"">
                        <b>Propery Forager Team</b>
                       
                      </p>
                       <p> +2348654978 </>
                    </div>
                  </body>
                </html>

            ";
        }
    }
}

