using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Tls;

namespace WinHue3.Philips_Hue.BridgeObject.Entertainment_API
{

    internal class HueTlsAuthentication : TlsAuthentication
    {
        private readonly TlsContext _context;

        internal HueTlsAuthentication(TlsContext context)
        {
            _context = context;
        }

        public void NotifyServerCertificate(Certificate serverCertificate)
        {
                    
        }

        public TlsCredentials GetClientCredentials(CertificateRequest certificateRequest)
        {
            return null;
        }
    }
}
