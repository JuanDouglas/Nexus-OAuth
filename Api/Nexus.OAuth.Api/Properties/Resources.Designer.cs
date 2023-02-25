﻿//------------------------------------------------------------------------------
// <auto-generated>
//     O código foi gerado por uma ferramenta.
//     Versão de Tempo de Execução:4.0.30319.42000
//
//     As alterações ao arquivo poderão causar comportamento incorreto e serão perdidas se
//     o código for gerado novamente.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nexus.OAuth.Api.Properties {
    using System;
    
    
    /// <summary>
    ///   Uma classe de recurso de tipo de alta segurança, para pesquisar cadeias de caracteres localizadas etc.
    /// </summary>
    // Essa classe foi gerada automaticamente pela classe StronglyTypedResourceBuilder
    // através de uma ferramenta como ResGen ou Visual Studio.
    // Para adicionar ou remover um associado, edite o arquivo .ResX e execute ResGen novamente
    // com a opção /str, ou recrie o projeto do VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Retorna a instância de ResourceManager armazenada em cache usada por essa classe.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Nexus.OAuth.Api.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Substitui a propriedade CurrentUICulture do thread atual para todas as
        ///   pesquisas de recursos que usam essa classe de recurso de tipo de alta segurança.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Consulta um recurso localizado do tipo System.Byte[].
        /// </summary>
        internal static byte[] account {
            get {
                object obj = ResourceManager.GetObject("account", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Consulta um recurso localizado do tipo System.Byte[].
        /// </summary>
        internal static byte[] application {
            get {
                object obj = ResourceManager.GetObject("application", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a &lt;!DOCTYPE html&gt;
        ///
        ///&lt;html&gt;
        ///
        ///&lt;head&gt;
        ///    &lt;meta charset=&quot;utf-8&quot;&gt;
        ///    &lt;meta name=&quot;HandheldFriendly&quot; content=&quot;true&quot;&gt;
        ///    &lt;meta name=&quot;MobileOptimized&quot; content=&quot;320&quot;&gt;
        ///    &lt;link href=&quot;https://fonts.googleapis.com/css2?family=Roboto:wght@300;400&amp;display=swap&quot; rel=&quot;stylesheet&quot;&gt;
        ///    &lt;title&gt;Nexus Company&lt;/title&gt;
        ///&lt;/head&gt;
        ///
        ///&lt;body style=&quot;padding: 0; margin: 0; background: #e1e1e1;&quot;&gt;
        ///    &lt;style&gt;
        ///        a {
        ///        }
        ///
        ///            a:hover {
        ///                color: #257470;
        ///                cursor: pointer;
        ///    [o restante da cadeia de caracteres foi truncado]&quot;;.
        /// </summary>
        internal static string confirm_account {
            get {
                return ResourceManager.GetString("confirm_account", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Isto é sério ?.
        /// </summary>
        internal static string DateBirthError {
            get {
                return ResourceManager.GetString("DateBirthError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a O token inserido não é válido..
        /// </summary>
        internal static string HCaptchaError {
            get {
                return ResourceManager.GetString("HCaptchaError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta um recurso localizado do tipo System.Byte[].
        /// </summary>
        internal static byte[] SwaggerDescription {
            get {
                object obj = ResourceManager.GetObject("SwaggerDescription", resourceCulture);
                return ((byte[])(obj));
            }
        }
    }
}
