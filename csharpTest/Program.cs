using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Yuduan.CSharp;
using Yuduan.CSharp.Models;

namespace csharpTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Get().GetAwaiter().GetResult();
        }

        private static async Task Get()
        {
            using (HttpClient http = new HttpClient())
            {
                //三代汉字
                var resp = await http.GetStringAsync("http://www.geetest.com/demo/gt/register-click?t=1527475267971");
                //三代滑块
                //var resp = await http.GetStringAsync("http://www.geetest.com/demo/gt/register-slide?t=1527479423500");
                //二代滑块
                //var resp = await http.GetStringAsync("http://id.ourgame.com/id.ourgame.com/captcha!lzCaptcha.do?t=0.6296260015670256");

                var json = JObject.Parse(resp);
                var gt = json["gt"].ToString();
                var challenge = json["challenge"].ToString();
                var result = await Geetest.Recognition(new GeetestInput
                {
                    //WebProxy = new WebProxy(), //此处传入代理（如果是用的代理）
                    Challenge = challenge,
                    Gt = gt,
                    User = "", //此处传入key
                    Referer = new Uri("http://www.gsxt.gov.cn/")
                });
                switch (result.Code)
                {
                    case Code.Success:
                        //识别成功
                        Console.WriteLine("challenge:" + result.Challenge);
                        Console.WriteLine("validate:" + result.Validate);
                        Console.WriteLine("seccode:" + result.SecCode);
                        Console.WriteLine("params:" + result);
                        break;
                    case Code.InMaintenance:
                        //系统维护，后续逻辑
                        break;
                    case Code.BadIp:
                        //IP被极验封了，该IP短时间内（预计封1-3个小时）不可用于极验3代。此处将该IP加入黑名单
                        break;
                    case Code.UserError:
                        //帐号到期，触发告警通知，提示财务或负责人
                        break;
                    default:
                        //常规错误，记录SDK返回的错误描述
                        Console.WriteLine(result.Message);
                        break;
                }
            }
            Thread.Sleep(-1);
        }
    }
}
