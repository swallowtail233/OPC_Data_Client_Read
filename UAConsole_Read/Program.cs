/*
OPC Data Client 範例程式
說明:  連接 OPC UA server, 讀取1個、多個 點位資料

請先下載並安裝Data Client 程式開發工具, 在Visual Studio設定好NuGet套件, 便能使用函式庫
執行程式前, 請先重建方案, 套用函式庫

Data Client下載連結
http://www.oneyear.url.tw/index.php/dataclient/menu-dc-download
教學 - 安裝與使用
https://drive.google.com/drive/folders/1BXGkkwC2C9dGUr9k-2GtP7TKYNb1RagY?usp=sharing

程式不能執行? 請別客氣讓我協助您!
-- Kepware OPC專精 壹年資訊 --
侯奕年 Derek
Line ID:oneyear
0932 832 233
www.oneyear.url.tw
derekhou@oneyear.url.tw
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

using OpcLabs.BaseLib;
using OpcLabs.EasyOpc.UA;
using OpcLabs.EasyOpc.UA.OperationModel;

namespace UAConsole_Read
{
    class Program
    {
        static void Main(string[] args)
        {
            var endpointDescriptor = new UAEndpointDescriptor();
            endpointDescriptor = "opc.tcp://127.0.0.1:49380";  //UA server的URL

            /*
            //設定帳號、憑證、加密, 若UA server有設定加密連線, 便需設定
            endpointDescriptor.EndpointSelectionPolicy = new OpcLabs.EasyOpc.UA.Engine.UAEndpointSelectionPolicy(OpcLabs.EasyOpc.UA.Engine.UAMessageSecurityModes.SecuritySignAndEncrypt, "http://opcfoundation.org/UA/SecurityPolicy#Basic128Rsa15");  //指定security policy
            endpointDescriptor.UserIdentity = new OpcLabs.BaseLib.IdentityModel.User.UserIdentity(OpcLabs.BaseLib.IdentityModel.User.UserIdentity.CreateUserNameIdentity("test", "11111111111111"));  //當UA server需要帳號登入時(不允許Anonymous連線時), 需設定UserIdentity, 設定帳號密碼
            */
            EasyUAClient.SharedParameters.EngineParameters.CertificateAcceptancePolicy.AcceptAnyCertificate = true;  //自動信任server的憑證

            var UAClient1 = new EasyUAClient();  //建立UA client實例
            UAAttributeData attributeData;  //存放1個tag資料
            UAAttributeDataResult[] attributeDataResultArray;  //存放多個tag資料, 陣列

            Console.WriteLine("開始讀取tag資料");
            Console.WriteLine("第一次連接需數秒鐘, 請稍等...");

            //一次寫入1個tag
            try
            {
                Console.WriteLine();
                Console.WriteLine("--- 一次讀取1個tag ---");
                Console.WriteLine("讀取Channel1.Device1.Tag1");
                attributeData = UAClient1.Read(endpointDescriptor, "ns=2;s=Channel1.Device1.Tag1");  //讀取tag, 第二個參數"ns=2;s=Channel1.Device1.Tag1" 是tag的位址, 請改成您需要的位址
                Console.WriteLine("讀取結果: Value:" + attributeData.Value + " Time:" + attributeData.ServerTimestamp.ToLocalTime() + " Quality:" + attributeData.StatusCode.ToString());
                Console.WriteLine("完成");
            }
            catch (UAException ex)
            {
                Console.WriteLine("錯誤: " + ex.ToString());
            }

            //一次讀取多個tag
            try
            {
                Console.WriteLine();
                Console.WriteLine("--- 一次讀取多個tag ---");
                Console.WriteLine("讀取Channel1.Device1.Tag1及Tag2");
                attributeDataResultArray = UAClient1.ReadMultiple(new[]
                {
                    new UAReadArguments(endpointDescriptor, "ns=2;s=Channel1.Device1.Tag1"),
                    new UAReadArguments(endpointDescriptor, "ns=2;s=Channel1.Device1.Tag2")
                });  //Tag位址, 請改成您需要的位址

                Console.WriteLine("讀取結果:");
                foreach (UAAttributeDataResult attributeDataResult in attributeDataResultArray)
                {
                    if (attributeDataResult.Succeeded)
                    {
                        //Console.WriteLine("AttributeData: {0}", attributeDataResult.AttributeData);
                        attributeData = attributeDataResult.AttributeData;
                        Console.WriteLine("Value:" + attributeData.Value + " Time:" + attributeData.ServerTimestamp.ToLocalTime() + " Quality:" + attributeData.StatusCode.ToString());
                    }
                    else {
                        Console.WriteLine("錯誤: ", attributeDataResult.ErrorMessageBrief);
                    }
                }
                
            }
            catch (UAException ex)
            {
                Console.WriteLine("錯誤: " + ex.ToString());
            }

            Console.WriteLine();
            Console.WriteLine("按下Enter停止執行");
            Console.ReadLine();
        }
    }
}
