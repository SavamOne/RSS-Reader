using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RSS_Reader.Utils
{
    public static class DescriptionPrettifier
    {
        static Regex IFrameRE { get; }
        static Regex AOpenRE { get; } 
        static Regex ACloseRE { get; }

        static DescriptionPrettifier()
        {
            IFrameRE = new Regex("<iframe[^>]*><\\/iframe>");
            AOpenRE = new Regex("<a[^>]*>");
            ACloseRE = new Regex("<\\/a>");
        }

        public static string Prettify(string description)
        {
            description = IFrameRE.Replace(description, string.Empty);
            description = AOpenRE.Replace(description, string.Empty);
            description = ACloseRE.Replace(description, string.Empty);
            description = 
            @"<html>
                <meta charset=utf-8>
                <style>
                body{
                    font-size:120%;
                    color: #333333;
                    display: flex;
                    font-family: Arial
                    flex-direction: column; 
                    padding: 10px;
                }
                html, body{
                    max-width: 100%;
                }
                img{
                    width: 90 %; 
                    height: auto;
                    margin: auto;
                }
                </style>
                <body>" 
            + "\n"+ description + "\n" +
            @"  </body>
              </html>";
            return description;
        }
        
        
    }
}
