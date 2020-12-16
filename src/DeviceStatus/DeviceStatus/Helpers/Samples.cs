using DeviceStatus.Helpers.api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DeviceStatus.Helpers
{
    public static class Samples
    {
        private static readonly Random rnd = new Random();
        private static readonly string defaultLicenseKey = ConfigurationManager.AppSettings["LicenseKey"] ?? BuildRandomString(16);

        private static LinkDALIdentifier MockDALIdentifier(LinkDALIdentifier dalIdentifier)
        {
            if (dalIdentifier != null)
                return dalIdentifier;

            dalIdentifier = new LinkDALIdentifier
            {
                DnsName = "Host" + rnd.Next(0, 999).ToString(),
                IPv4 = rnd.Next(193, 254).ToString() + "." + rnd.Next(193, 254).ToString() + "." + rnd.Next(193, 254).ToString() + "." + rnd.Next(193, 254).ToString(),
                Username = "User" + BuildRandomString(6)
            };

            return dalIdentifier;
        }

        public static LinkDALRequest PopulateMockDALIdentifier(LinkDALRequest linkDALRequest, bool addLookupPreference = true)
        {
            if (linkDALRequest == null)
            {
                linkDALRequest = new LinkDALRequest();
            }

            if (linkDALRequest.DALIdentifier == null)
            {
                linkDALRequest.DALIdentifier = MockDALIdentifier(linkDALRequest.DALIdentifier);
            }

            if (addLookupPreference && (linkDALRequest.DALIdentifier.LookupPreference == null))
            {
                linkDALRequest.DALIdentifier.LookupPreference = LinkDALLookupPreference.WorkstationName;
            }

            return linkDALRequest;
        }

        public static string BuildRandomString(int string_length)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                int bit_count = string_length * 6;
                int byte_count = (bit_count + 7) / 8; // rounded up
                byte[] bytes = new byte[byte_count];
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes).TrimEnd('=');
            }
        }

        public static int BuildRandomInt(int numDigits)
        {
            int output = rnd.Next((int)Math.Pow(10.0, numDigits - 1), (int)(Math.Pow(10.0, numDigits) - 1));
            return output;
        }

        public static LinkRequest BuildLinkDALStatusRequest(bool buildLocal = false)
        {
            return new LinkRequest
            {
                MessageID = BuildRandomString(rnd.Next(5, 16)),
                TCCustID = rnd.Next(1000000, 1999999),
                TCPassword = BuildRandomString(rnd.Next(8, 16)),
                IPALicenseKey = defaultLicenseKey,
                Actions = new List<LinkActionRequest>
                {
                    new LinkActionRequest
                    {
                        MessageID = BuildRandomString(rnd.Next(5, 16)),
                        Action = LinkAction.DALAction,
                        DALActionRequest = new LinkDALActionRequest
                        {
                            DALAction = LinkDALActionType.GetStatus
                        },
                        DALRequest = buildLocal ? null : PopulateMockDALIdentifier(null, false)
                    }
                }
            };
        }
    }
}
