using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Microsoft.WindowsAzure.Mobile.ResourceBroker.Client.Test
{
    [TestClass]
    public class E2EBrokerTests
    {
        const string AppUrl = "<INSERT APP URL HERE>";
        const string AppKey = ""; // set to try end to end
        const string ContainerName = ""; // set to try end to end

        #region Test image
        static readonly byte[] TestImage = Convert.FromBase64String(
            "iVBORw0KGgoAAAANSUhEUgAAAIoAAAB2CAIAAACYtZxRAAAAAXNSR0IArs4c6QAAAARnQU1BAACx" +
            "jwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAB90SURBVHhe7Z1prF5V1cedUNGYaIyJMW8MURMx" +
            "xmg0Th/5ZPSDwxcTv/nVRDDGyKgoUIodoIOtdtC2Wm4BBdqinYulrZW2CJQOFGmB0hZKS2sLivPw" +
            "/s7+7bs5rOc8zz3PvfdpL+/LPys7++y91tpr2Guffe69hVf99xVMYLyc0nPqf943LpTVvRwwcdMT" +
            "YjpQyktOPEys9ISonRPKpkwMTIj0hABNEMKw/yTYSZaebZzL9IRwTFjK5p4LnJv0BP9fFpRNP7s4" +
            "q+kJDr9MKTtzVnCW0hM8/D9A2bEBY+DpCV6NhXg///Of//zXv/5F59///jf9v/71r3/72994ZJDH" +
            "v//974yAF1544c9//vOfEp5//nnaM2fO/PGPf3zuuecYD2rHQtnJgWGw6QnO9EtZSwI5ECTGZPzj" +
            "H/+gJT2nT58+derUM888c+TIkSeeeOLgwYO0hw4don084bHHHqN1EJ6nn34a5hMnTpi8sGi/lO0b" +
            "DAaVnuBDX5Tz8FKQFdSSD0Cf3FAKRPnw4cME/amnnjp58iSVwTjM2iB8pKSoHqAICXs0gVzyyCDi" +
            "zAZL2pOrhKXHjvFPT7C7JSHY20PGneIEo1aoAPDss8+SEgZNHpDNCrPIADkrDIBxRkwYiXnkkUd2" +
            "7dq1d+9eUoXO48ePozaY15LyAuOHcU5PMLcNKWjoS0s0bQkuHUBWeOQco1YI4l/+8hemYDYT8rdH" +
            "tWSqRVpSxSmHTvJ033333X///QcOHDh27BibIJjahtQ8XhjP9ARDe1OWSTBeBg4Q6wLGq/2fXvse" +
            "Spxp8jBCYhQBRU9vwIOsbWdVUU+8pR544IGdO3fu27fvySefJE+jKKasccwYn/QE43pTlhlGiawg" +
            "XqAUhB3uXUSKg4htXthsfdn42KmtjjJbh1MADazlIPcFCohK4tBjQ3CbGEUxqWqMGIf0BLN6UBbo" +
            "QI7QcG4AkbJoiD6bl9zw6oYBZhnkl8HBpKkV1ENr7unTumKlenhbcLUjPVu2bOG4e/jhh48ePUqC" +
            "+rrppdXGhLGmJxjUg7JAB6owJxgaQGgKiBFnmu9/Hpk1iDySGPpFQ6WrHWSmRU/p1ztlnHU56zZt" +
            "2rR161ZOPK7m7BXQ/uMJJWPBmNITTOlGmbsLjDUtMCWC6HOacWPmikWwyIS3A9loGZSziuio0mOn" +
            "rlNtDLJc+eDlcNu2bdv69espI7LFI4ctx2xwsxtVS44Wo0xPsKAbZe6eSFGqYFkYF4LCBZfd6quF" +
            "FtQzYTTLo21LFJECx0UeSmAVwCD3us2bN69evXrPnj18LbFp+soQpPJ+MZr0hIW7UeZuAQJNS2KI" +
            "BSVSuX3qFCHgkdIxQzDYdqLbeHtUqRhGHhpGGeSYXbNmzapVq158FXW43JtU2Bf6Tk9Yshtl7hbQ" +
            "f0FuaNmY3Jf8shEWSimXAJWorQ2KSOmk4YwyKFgUsFFoKeibb7557dq1Dz30UPC3JblEe/SXnrBY" +
            "I2XW1shhSCAKZIW6YW/yaGiAs3YUUXagKGuxbulwuA0NDQWX+6Kkuy3GOT2Zr0/ov28XvjDKK4e6" +
            "cduWSCX2swdWBBigDSK4PArK2lugj/SENTop83UHvtnWOwXcVvlo5xpNy6yJMS71tugZNFyRDruE" +
            "7UKHx+Dy6Cipb4W26QkLdFLm6x+GgNvaCy+8wLWVO5JxcdxM0CdGXqyLyKDBKlrCRiE9wd9uZMWH" +
            "wU7Ka4yEVukJqjsp840EY01rR/8BLvHKOX369COPPIJvTJWfpxWegqJn0ChrgeBvN/JMBpgdpjrJ" +
            "VXpj5PQEpZ2U+VoAo21xgI4JANyeSc/Bgwc91mAAnidyniu4evC3kfiC5mOAXaWUrgWeTkqL9MJY" +
            "05OZ2kFvaYk+HdOAJ88///zJkyf37dvnuL5RUnWRgjI4aLhK8LeRdu/ezXUGF9hkSOkXoB84A7lQ" +
            "D4yQnqAuUGZqjRJcEkAHB+jw1sExjjVKp/hmR55K8hwh+NtIfAbde++9jz76KAXEPvPdA3QW+wN/" +
            "IBfqhl7pCYoCZaZ+gMVC62k5wf70pz/xobN37176ya+cORgUSaLnAMHfRpoyZcodd9yxefNmTuYj" +
            "R47w+mS34QhJ0gURpALl9ZowyvRkjj5RCqLkwB/h4Js/wnGEVhSRs4zgbDfiG+DGG29cvHjx3Xff" +
            "vX//fj8JeI/igluNxKBNF4JsndKazeianqAiUGbqE24l4ObCDfDss89ysrHpGHRKx4Aiyp41BE+7" +
            "EYZxlJGbOXPmrF69evv27WwyfOHzwNzgYHKiwohqXboTo0lP5hgVSk2QBl6k4NChQwcOHKjPnkME" +
            "TxupRJwc3HnnndOmTVuxYsWmTZu42vj7XPYcs/iiOzKDoCeQPAHN6QmSdcoco4KOAfr4QG44rB97" +
            "7DG8YhBnnC1QJImeDQRPG0l7MNVa37BhA6+fX/7yl6SH1yff1JQUJwFT5XCruxC01SlzvBTnJj2A" +
            "9ODJ8ePH//CHP+iP6cmsZx3BzUaCzYIg9OwtOjt37pw8efLQ0NDGjRt37drl7YAp2Io7daeCwkCZ" +
            "qYaG9ASZOmWO/lFM1D1a0sOL9OjRo1SPI/DUPWkD+RUU9cfGKdpOBDcbSc5iv1uKivnBD37AG2jd" +
            "unX33XcfBzUfCbx+YGisHhDU1ilz1NBHevL0GIChuueLh43G65QCYpwRxp3tF4iD0q+3daT5iOBj" +
            "N8rcNc1En87jjz/Ou2fhwoXcDnbs2MFW8y8RmPX0KyJ1BOWF8nQNMT1BoE6Zo39gnyD6dvzcOXPm" +
            "zMMPP8w2xBngVJZphzp/EbdjvgGazbrjDorgYDeCMwjyqC9PPvnkTTfdNH/+/FWrVvFxyh2H3YZr" +
            "rK5HMitVEPTXKXMMo2168nT/0LgSIDrYTUooHU5qXjwMMiJPpye9AT9QJ3DD0il3J84ZOdkQjvgI" +
            "goONVEyytSOcOnHiBJ8+s2bN4vK2detW3OF2wKK90wPCQoXy9DBekp7AWqfM0Q+KWXQELgn/vpmT" +
            "rfxWVAb52wNBWr/S6dCaBtpt27bNmzdv0qRJ3H3Z0WwIV6nE2uWG+KJHKReqg1lajrLp06fPnDlz" +
            "+fLlv/3tb/mA4/vaT5+yXKNfYa06ZY6EAaZHYJzA3AJePLxCOdk4B3h0VmalWiIpznDDoodrOon5" +
            "/Oc/f+GFF773ve/99Kc/vWjRosyU9AfXGklmbUNE84DjwOIgPVTPjBkz6tXDu2fU1QNljoRW6cnT" +
            "fUKzdIxWYDTAAdwoLx7GYQZ0kmgrBP0eaKi94YYbPvWpT73jHe94/etff955511wwQVf+9rXeEnI" +
            "FlxrpBJcQN8O4qKsSMvhVq+e+uEGg1J12TrCooXydMKL6QlMdcocfSI5VQFDQcpLBQr/ueee4w76" +
            "xBNP8OgsbIoo2xII2qKHDgfmD3/4ww996EPnn3/+6173ule96lW0b37zmz/3uc/t2bMnONWNiKxH" +
            "E0jmZ6QFX3TKFTk2ublZPRyn9epRXJEkGhHWrVPmaJOePN0/sM+2ykkNmM6dDU/wTTdAcrmCsm0g" +
            "P7JGk/bWW2/9xCc+QdGQGPCa17yGlscvfOELwaluhFpMAnTQqXn2qyVTx748HG6kh8sb1WN6yrsH" +
            "T3XHthFh9UJ5eqDpSdGroEsCr7AerzjZ2OyMmDA6iNi2hMyIc6yxCvXxpS996Y1vfOOrX/1qsmIL" +
            "3vSmNwWPGgk9aOOwmjx58iWXXEKseTTxelEt2VE9nAF8lnJzu+OOO7waHDt2jBcqswoqkkQbEGwo" +
            "lKcHmh5gBLGv2OrPcnzxcMQxQm6IArOiEmsH+dWMKnbxu971LhMDLB0Q3GkkbADcJ6dMmfLBD37w" +
            "3e9+91e+8hVeV2g2DXTKorQsCj+d3bt3czmcO3eu6aF6/KmoVsls24hgRqE8fXaqB7ibACHA+sOH" +
            "D+/fv9+fizgOPy2PyrZBUlyJ07///vs//vGPv/a1rzUlpXSCL43EjmF/qIQ7xdvf/va3ve1t73vf" +
            "+7iRox/LXSitmRdl0JLdvn379773vQULFqxcubJ8lvpTAzkVUbYTwZI6yZDTE+YKOTsWlNADjOZk" +
            "Y6c/loDduiFP5U0/6VEnIqT5+uuvL6+cguBLIyGODQBVXCve+c53WnYk6eqrrybQLlQMswO/Fb9m" +
            "zZorr7xyyZIlv/71r8sPdfAxmZalimwjgj2FnB1gejCrwBDgEqXjXxZwvjHOESEDyGKtgQg66XAK" +
            "8XFDTI0spRO86EZoMIjo4UX41a9+lZuelfeWt7zli1/8IqVQWZbgovDTYraCt9xyC+kZGhpau3Yt" +
            "xcer6NSpU3zVMYVOpYpsI4JJhZwdePVonLnhQKB0SMzevXt98TCun7YytwTMvgC41HIcEVMPt+BC" +
            "N0KcRTEAoISbxcc+9jE0mOO3vvWtF110EVf/ZFSFtOaL7tChahcuXPjd73532bJl69evf/DBB/19" +
            "tr9QkAdm+km0GcGqQs4OtnpsAbYSSvwhK/jAG9Wz2+rBAX3o7UkAzEn3f7797W8TUEBkg/2NpBRQ" +
            "g5ud+HKyqcQkffSjHyVnlWWJzUXt2OLL7Nmzr7vuOtKzcePGhx566OjRo6dPn+b9CkORKrKNCLYV" +
            "cnaA6cE+Wg313UtK/MchBw8e5BE3SJuzsMlZSbaDzNwDP/vZzxJWzqVgfCPVZV3RjTJ//nwOtJTl" +
            "jAsvvJAqh63UAYCfviMcZbyf5syZw/fW1q1bvVXjoPmGuUj1QDCvkLMDrx79oUpwCbs5mrlS83nB" +
            "IHFh1sIqnEm0FYwRL56PfOQjRDNY3kgKIlWAEsDg9OnTzzvvvOpKPnzre//7309BwFM3jD6tRb9r" +
            "166rrrrqxz/+MXe83/3ud35oU1JsO9mUKrKNCBYWcvZsHG7YCnzxsL/w2WKyaJgyQHQqsdYwRmzw" +
            "Cy64IJjdSIiUSLmWS9OCK664gpRQghxrZugDH/gAhzBsSBVB+4D+6tWrL7/88p/85Ce/+tWvuFVz" +
            "JLDtyt8aANQq1QPByELOno3qMY6UDucyBwLVU8arwAy/RUUSbQVrLhjcjbL2jii7NO3FF19MSiwg" +
            "bwd8SD366KMwYyE8RZBHOuw27wXeqnfu3Mk9grOBu7inZV2qB4KdhZwdbHoAQXSHYjfW47C/43GQ" +
            "1g4jiOi5/RGBYLC2G1GmMCOifuGirg6+/vWvWz20Vg+vtPInRAAR+7SAKS4FU6dOLf+ckSuPL57y" +
            "KnWhERGslZwa+NWArWQIqHqqh7sQHX1wttQQj+1dAsHURkI5CwFsKCGzdUVaLbnkkktIiXVj++Uv" +
            "f9nPUgWB/IA+Z8A3vvGNuXPnci/YvHkz9wLS44tHhbDRtkGwWXJq4NWDlYSGD2lcpfwff/xxHnHA" +
            "rFhbQn4F1dADwc5GMkBoQ7mdJFqhWimN+xqn7+28/u5hxDtYsi4XX5Kr9Nx+++2XXXbZokWL+DIt" +
            "9wI+ut1wsoG0Wi8Emws5O8D0AAOEueSG77X9+/fjAyN4a2JgsE8H6A+dSrg7gpGNBFs9QHZKK8q6" +
            "DN5www2khHePPxx6wxvewJUMnqr0hovPJNHy8r/pppsmTZrEycaLh3sB2+7k8H9QTr/gtO2NYHYh" +
            "ZwdePQCL2aRPPfUU6eFkc4TQFPCIJzIrqIZGBAsbCbYSmk61jgB4WN2gDw0NkRU+SC2d97znPdu3" +
            "b4fHc1jOYiTnGFfqGTNmUDp88fz+97/HO39eoMK6O70RLC/k7MDfPdiKxVQ9+4t7AX2MpgVMCfp1" +
            "f2w7EWzrRkUVyJId0DbachYRZX9XRAFxvl100UWUAgxCa+nAj+DGjRu/9a1vzZs37xe/+IUfpE8/" +
            "/TQvHqpKd2g1AKQFuyIYX8jZcUuPdmgQKH38Ad7Z/DEoprMfAW4It6f8RVVAMKwbwYlCtNGxbYRr" +
            "AfpUNi228XlLes4//3zSM2vWLAYxDGvtoE3LaefMmcOV+qc//enKlSvvuecevhYQ92RDJwx1A1yl" +
            "G4L9hZwd7OFGi8WUPK+cffv2cUHA9JISW0D+dEmRzrAGqxoJKRSiQT0iy3eAKdloSQ+r0/fLFHzy" +
            "k59kPzGCwZgHP5rh8XuTC86VV17Ju2fp0qXr16+vv3jggRmdKhRpwa4IXhRydoDVg5UAl3jf8K3D" +
            "CcA43poVUmLfRziLoG1BMKmRjIuChqY8NqIwsC4dbKBPEXzmM5/58Ic/zFuHQfQADas8SYmkz1cO" +
            "JxsXB062LVu28KlAevCxpFlmQF9US3ZBcKSQsyOkB5JhRGCNbXEJmJtn0z+w4pONKJAVZhnHGdqU" +
            "nXxSI25bRzCmkeTslO0L2o9VgA7aMIx9g220POoX9TRz5kxOtgULFtx11107duzwZzn1g0F35C+a" +
            "GxEcqZMMOT0gTBfK0yPB6NBiDZbRp8OxxqWAXXngwAFLBOeZJUnkhlZ/AFPIFiWVxta5UVCURzuV" +
            "libI0wksMazaQ4vZ3sfoI8j3DXe2KVOmLF68ePXq1dzZKJ2SHpGkXzzfFGxE8KVQnh5cemhxqXzu" +
            "HDt2jFkGSQ/jIT16UpSAYEM3knlc4NJYQj6wjY6D7ipa7ORx+fLll1566fz585ctW3b33Xfv3r3b" +
            "X8HhV3EcwCmK5kYEdwrl6XFPD8A4ncFJ7pp8EHCyWTRM0Zob0C09wYBulFYbNxhTWuyhZYRHwCN9" +
            "LWS3TZs27fvf//7Pf/7zFStW+MMC7mycEGZRHxUUSXdXBI8K5ek26YEyR09kc2r3Zur99OnT3HO4" +
            "F+i245wVZAuQIR5DesLSjeSK4wtTovFmyBFMoqN57LPLLruMk42vUS4I5b8u6slW51RWONKJ4FSd" +
            "Mkc9PSAwFcrTPaFBtAD3qAzTw+cOlwJNJzFsQMa7VU9Yt5HSauMPDNBITaLDI4ZpHi083AUuvvji" +
            "GTNm3H777Zxs/pSaKzhbrXK7Vj22QOWNCH4VytMJrdIDZY7u0BqAJ8adrHCrwQFsTWVTnel4QpIs" +
            "oJIh0xNWbCTYXMtFxxdBLY9VyFPCzNDUqVOvueaaH/3oR+TJfyn/zDPPlEuB/O0tDK4VytMJ45Me" +
            "rCnQE4xmW/mXujCQGzIBSAm5YbZkSOfDco3kWgMCltsGMIh5tLxj+Bq98cYbf/azn61Zs4Y7myeb" +
            "pSOP/G0QXKtT5kh4SXpAYC2Up7sDE20xlDSQFV45XDrNlumxdOq5YTAs1I0MFmAV23FHUR70WxN8" +
            "gX7zm9+cM2cOJ9tvfvMb/y4HN/ECH+WxRSRo6ETwrlCeHkbb9ECZowlYI7APcGHD7vLWSQdbdbIB" +
            "M1TSE5boRqo1Cixne3aA/VYGRcOVet68eStXrvTHoHzu4Ck7TNtkTkIjIHhXp8wxjJgeEAQK5elh" +
            "aAotKFGjUAB2cyhzNDNl9YDO6gn6GwmdBWWTFhQzBgGXcEWuzrx4Jk+e7B9+7Nixg5ONLYgvuAaD" +
            "LcjCPRF8LJSna+gjPVDm6ECJGlXC3YwLGx8E5a1jbqgVcwMDnaC5kRA0JYK+/tsOGsmhHHdq5Tvf" +
            "+c6sWbOWLl26YcMGTrYjR448n/4oR9tgQ4SOsj0QfKxT5qihv/RAmWnY+gItw1zqnerZv38/0QTY" +
            "7clGuZTqCTobCZ1qKEBVCkUFF9WMZE4fKIKNKAysQsvGot24cSNfPP4YlJMN7/wNgicb/HWTeiO4" +
            "WafMUUNDekAQqxOzGqH1tILwMU4CTp48yVuHlkECCsiN1WN6gsJGchU1q6R6fQ1/wwKnihmDAJqx" +
            "wdUpGq5tixcv5krtn7RxepMeZ+sB6Y3gZp0yx0vRd3ogebCpgHhhpVWC6YcOHXKElsQIchP0dCMU" +
            "oh9v1VAFYPjuVxLjupqRzOkDRbARdQYX4gth5syZ11577S233LJu3Tr/pO348eO+eDCpEhtGtUAX" +
            "BDcDZaaXojk9IAgHggFTsN4OVpIYzOWtwzHtWyeFsfrdAWifGxUWtSCVTb74EQszxGxho+0LRVAL" +
            "K0UJ9gMDHXbb1VdfPW3aNK7UmzZt8sXD8YA9ScFLpHogeFqnzNGBrukBQUWdMAVoGSB85Ia3Dp5g" +
            "OjEliGigZcvjRhBvJFUB1dIii+aq9BLQo2bQPiidQNa2qBJFbWkZobNr1y5ePLNnzyY9mzdv5lub" +
            "Fw8bEdvQU/hFWqEBwdk6ZY4mjDI9ULGeDob6azdONpLEIHGkddcHwUaqPBuG3qIcWbNC8dmOS3qK" +
            "INqKQkCfwQA416xZc8UVVyxcuJAXz7Zt2x5L/wE6rtqIwFC3xE4ngrOBMlMTeqUHBEWB8AcebKJD" +
            "Vo4dO4bpuioIbhBpJDxEj60KfSQlZkVwrWAhxkEKRQ5KEuoDRRBtRSGgX9fJCC2DS5Ysueqqq26+" +
            "+WZePNu3b38y/V/suBfIkERfYn9AcDZQZuqCEdIDgrpAWE8OCBxnMddN+oiYG0onMDcSXhGOOhgp" +
            "GsiKn0q0rIJ+BlmUWThZy7YvFMEqrrULiG0ZpwM4xPggBUNDQ5TRAw88wAHuXxbIDCpbhy8ILlEQ" +
            "nO2kzNcFY00PRMgoHX+vAz8m4lvg6UZ6hRQitkBvcd6zkcRwkvgxy2PJkIKdERkRRbCsBdRJqw0C" +
            "Nl4z11xzDTe32267bf369Xv27Dmc/l92WAK/PLRosENbEJztpMzXHSOnBwSlnUSx862DxTBjYpjt" +
            "RrhXPCRYBXoLyASblFiwW8kQeRrf6lEPSIZUxwDKGRcuwT3t8ssvnzdv3vLly++55x7c9MUDswyo" +
            "Sjpi9QRnOynz9USr9ICgupO4F8DWMjdEwVgUrwDidlBi4GCgYogF8Hwb3/SgJOWlKprSsjpglj5s" +
            "GzZsuPTSSxctWrRq1SpePOVP2tiLsKlHkaIZBH87SbYR0TY9LBwW6CTcCyONRMThLLFAOS36C4ia" +
            "gSMZpsfqMSglr3UpjewBeWRWkJYlgMYANDsiAyN0br31Vu4FS5cuXbt27YMPPsi9gK8634LYkPRl" +
            "lFWCv51UGdQObdMjwjKjIIvAcBAIHQMEAv20BThf0sO7jQw1Vg+cybQ+oCCtm0BjALl3URnooHzx" +
            "4sXXXnst6eFesHfvXu4F/qDa2aQvm20n+NtISrVBq/QYPtqwTL+EV4CIV1s0hViv0FwH4wAG4gU/" +
            "iSE9JIn+eFWPYAmzolqQ7MrnG0tg6owZM6ZPn86teuPGjXyQ8tF95swZ+GVToUB/8LeRkjlt0bZ6" +
            "ijVhsfZElCkdHMY3nVchypN3GQyyFlEjWN4LLB0PN8QZREOVwOHs2vZG4QQIsgStuUEnifeizAg2" +
            "0AHw85q57rrrZs+ezRG3ZcsW0uO9gFlVCTiRCv42UmVKP+ivekRYsg0ZX0vH4OIhLdqKfjq0TtXT" +
            "Y/XQGkTEmYKtblIysxfkkdlVgKuYHtTyaNydhZNauf766+fOnVv+oUhJDwzFgOBsN9KSvtA2PQKz" +
            "cAOEhXsQkeU0MD1l7xsC3avrd9yoGTirR9DvTI/iycxeKJzAVdwEaCvVY3rUTAs/L5tJkyYtWLDg" +
            "zjvvvDf9T3rqf6+rAcHfbpSs6BttDzdMscUNwMdaWL4HkR6PJgQNrv7rHmptHSRAJXAEAkHES4KL" +
            "BniCeG/AU1BWQY+raBuPZggeWqS2bdtGepYsWXLXXXft3LnzYPp/jJDOJP3P4GYP0oZRoI/qMaC4" +
            "gYlcYPwXMMGObuTRZPUQAgOkwqIfmCEYiBSc8HuynT592upRAzFVFhHFk5m94EK0AnGAHrSh1so2" +
            "8awOAy38XKY53IaGhlavXu3/z5y3ESLIBgd7UFp/lOijenCDEBMprUxu9lHdkJHVfzxUHOW0ghHS" +
            "5qY2cKSH0mFR2nGvHlSRFVbRNW3TPGaRWrFiBdWzbNmydevW8dHjrTo41YNceixolR7MxXQsJkxP" +
            "pP+HHX2jA+gHs3oT0UEngioH6gFMER1zQ7ysG8qUoHi4MQXGkh5DbxpchdxYPYyrVjOQ4libOnUq" +
            "17ZNmzbtaf3f8pVcd4xolR58wDFiRG7Kf8qU8bq3wbgRCVmVVzFLeujTosrq6UwPcSSgnj8YALOy" +
            "QFU9UPQjCLAZPZYO7qhZtWiDDQY6fJNOmTLltttuC8aPSC46drRKD3bjA6V9+PBhAqf1eKK3JaB4" +
            "GKxsQ+hHTwpyPnZURew82cgNSSJVVg/LwVMMUDyZ2QvqBxqPEncAsC5Zrl6UKEcqmNqGXG680Co9" +
            "hIbccFvDGR6xPsXnxXOgHtBgbktCbQmcqgxcSY/VY+nAZm5SwCskM7siMyVosPuJJdCJZkC/5B7N" +
            "wbyWlNcbP7RKz4EDB6gbokYfBwB+Gk1QcoOHJBIQ02B3e0K52ggcqkhMSA8rEmLTgz22I0K2enpY" +
            "BYWuUtITjGlPrjLuaJWeo0ePYjq+0TcldEwPrtbTg5/kRlSvjQ43+iK1+e4hiPXDjdWrWhhGMrMr" +
            "MtPwliI3AFUYjM6w6CgoLzMAtEoPztAaEVp3bvGT3OAn2xBXiaObHYyxjDqJVchNqpz4k/zeyHzD" +
            "6Qlqx0J5gYGhVXp0zJbQWDSADQg8iMgNm52d7mYnMTwCZuEJXg2Csq3j8VuPNsRCBMQVB4dW6SmH" +
            "ST091E09N4ByKdVjehj0TAeIBA9fpoQjJmaipAeDTEwoHd835IBM5KPs1ClPtpIeq8f0IG6ag8Mv" +
            "Fzpx4gR7EUcmVnoGhOD8hKVs7rnAuUxPQQjHBKFs3DnFhEhPQQjQOaFsysTAxEpPHSFqA6W85MTD" +
            "xE1PQAjoGCkrnfB42aTn/ydeSc+ExivpmdB4JT0TGq+kZ0LjlfRMYPz3v/8LPRvRPN1TSq4AAAAA" +
            "SUVORK5CYII=");
        #endregion

        [TestMethod]
        public async Task UploadToBlob_BlobFileCanBeUploaded()
        {
            Uri dummy;
            if (!Uri.TryCreate(AppUrl, UriKind.Absolute, out dummy) ||
                string.IsNullOrEmpty(AppKey) ||
                string.IsNullOrEmpty(ContainerName))
            {
                Console.WriteLine("Fill the AppUrl / AppKey / ContainerName fields to test this E2E");
                return;
            }

            var mobileService = new MobileServiceClient(AppUrl, AppKey);
            var fileContents = new MemoryStream(TestImage);
            var url = await mobileService.UploadFileToBlobStorage(ContainerName, "ghost.png", "image/png", fileContents);
            var http = new HttpClient();
            var resp = await http.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            Assert.IsNotNull(resp.Content);
            var blobContent = await resp.Content.ReadAsByteArrayAsync();
            CollectionAssert.AreEqual(TestImage, blobContent);
        }

        class AzureResourceBrokerStorageClient : ResourceBrokerStorageClient
        {
            public override async Task<Uri> UploadContentToBlobStorage(string containerName, string fileName, string contentType, Stream fileContents, string sasTokenUri)
            {
                var storageCredentials = new StorageCredentials(sasTokenUri);
                var storageAccount = new CloudStorageAccount(storageCredentials, true);
                var blob = new CloudBlockBlob(null, null);
                blob.Properties.ContentType = contentType;
                await blob.UploadFromStreamAsync(fileContents);
                UriBuilder uriBuilder = new UriBuilder(sasTokenUri);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                return uriBuilder.Uri;
            }
        }
    }
}
