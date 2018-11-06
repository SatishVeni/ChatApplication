using System.Web.Mvc;
using System.Web.Routing;

namespace SampleChat
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            string lic = @"eJxkkFtTwjAQhf8K4yujaYtgcZaMUKpCuRSwKLwFGiHYJjWXFvn1ogLeXnb2
7LeXMws9tqRc0dI2TbhqnJHVuRLPuiCSXidf6AxDKEVslroT44k2MROAvisw
MoRrpt+wDeiUg2eUFimVGAYkpdjPSWKIFhLQpwZPpBnhb0fABC8drAA6MvBT
whKsSELVzQ9nF/G+6Yvtm0+HoiwmmvrbjEna3mfYsey6bTkVQP8QdFSbpgJr
afa7DgI+4u9517YdB9AfABO24kQbSfHCevEj3WXdq9ZoNy5ClTRrRdbxml45
DucVvmyxYLOdFy55mkWWWjmoT+qjYV41eeDevdbsSfnBidZlOW/vLjdttO7J
ASNWB7l1P5PCmVwuinA4NsOATjMVDXubWb94eJxSw2Zq3F2KW3o7jfvNIMwj
uV2PNPc2wl1Mq8pBzd6aqxaq5sHuntX6DUDfvgEd3o3fBRA=";

            Matrix.License.LicenseManager.SetLicense(lic);
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
