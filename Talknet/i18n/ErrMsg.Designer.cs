﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Talknet.i18n {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ErrMsg {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ErrMsg() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Talknet.i18n.ErrMsg", typeof(ErrMsg).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   使用此强类型资源类，为所有资源查找
        ///   重写当前线程的 CurrentUICulture 属性。
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
        ///   查找类似 Already connected. 的本地化字符串。
        /// </summary>
        internal static string AlreadyConnected {
            get {
                return ResourceManager.GetString("AlreadyConnected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 An exception occurred while a command handler is running: {0} 的本地化字符串。
        /// </summary>
        internal static string CommandExceptionDesc {
            get {
                return ResourceManager.GetString("CommandExceptionDesc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Argument {0} cannot be empty. 的本地化字符串。
        /// </summary>
        internal static string EmptyArg {
            get {
                return ResourceManager.GetString("EmptyArg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 {0}: {1} 的本地化字符串。
        /// </summary>
        internal static string ExceptionDesc {
            get {
                return ResourceManager.GetString("ExceptionDesc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Stactrace: 的本地化字符串。
        /// </summary>
        internal static string ExceptionStacktrace {
            get {
                return ResourceManager.GetString("ExceptionStacktrace", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 At least {0} argument(s) expected. 的本地化字符串。
        /// </summary>
        internal static string ExpectedArgCountGe {
            get {
                return ResourceManager.GetString("ExpectedArgCountGe", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 More than {0} argument(s) expected. 的本地化字符串。
        /// </summary>
        internal static string ExpectedArgCountGt {
            get {
                return ResourceManager.GetString("ExpectedArgCountGt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 At most {0} argument(s) expected. 的本地化字符串。
        /// </summary>
        internal static string ExpectedArgCountLe {
            get {
                return ResourceManager.GetString("ExpectedArgCountLe", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Less than {0} argument(s) expected. 的本地化字符串。
        /// </summary>
        internal static string ExpectedArgCountLt {
            get {
                return ResourceManager.GetString("ExpectedArgCountLt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 No arguments expected. 的本地化字符串。
        /// </summary>
        internal static string ExpectedNoArg {
            get {
                return ResourceManager.GetString("ExpectedNoArg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 An exception occurred and command handler failed to handle it, which is fatal. 的本地化字符串。
        /// </summary>
        internal static string FatalException {
            get {
                return ResourceManager.GetString("FatalException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Inner exception: {0}: {1} 的本地化字符串。
        /// </summary>
        internal static string InnerExceptionDesc {
            get {
                return ResourceManager.GetString("InnerExceptionDesc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 No data available. 的本地化字符串。
        /// </summary>
        internal static string NoDataAvailable {
            get {
                return ResourceManager.GetString("NoDataAvailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 No inner exception. 的本地化字符串。
        /// </summary>
        internal static string NoInnerException {
            get {
                return ResourceManager.GetString("NoInnerException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Not connected yet. 的本地化字符串。
        /// </summary>
        internal static string NotConnected {
            get {
                return ResourceManager.GetString("NotConnected", resourceCulture);
            }
        }
    }
}
