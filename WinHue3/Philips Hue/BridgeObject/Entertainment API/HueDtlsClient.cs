using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Tls;

namespace WinHue3.Philips_Hue.BridgeObject.Entertainment_API
{
      /// <summary>
        /// BASED ON Bouncy castle test Mock DTLS Client and Q42 DtlsClient
        /// </summary>
        public class HueDtlsClient : DefaultTlsClient
        {
            private TlsPskIdentity _identity;
            private TlsSession _session;

            public HueDtlsClient(TlsPskIdentity identity, TlsSession session)
            {
                _identity = identity;
                _session = session;
            }
            
            public override TlsAuthentication GetAuthentication()
            {
                return new HueTlsAuthentication(mContext);
            }

            public override int[] GetCipherSuites()
            {
                return new[] { CipherSuite.TLS_PSK_WITH_AES_128_GCM_SHA256};
            }

      /*      public override IDictionary GetClientExtensions()
            {
                IDictionary clientExtensions = TlsExtensionsUtilities.EnsureExtensionsInitialised(base.GetClientExtensions());
                TlsExtensionsUtilities.AddEncryptThenMacExtension(clientExtensions);
                TlsExtensionsUtilities.AddExtendedMasterSecretExtension(clientExtensions);
      
                return clientExtensions;
            }*/

            public override ProtocolVersion ClientVersion => ProtocolVersion.DTLSv12;

            public override ProtocolVersion MinimumVersion => ProtocolVersion.DTLSv12;

            protected virtual TlsKeyExchange CreatePskKeyExchange(int keyExchange)
            {
                return new TlsPskKeyExchange(keyExchange, mSupportedSignatureAlgorithms, _identity, null, null, mNamedCurves, mClientECPointFormats, mServerECPointFormats);
            }

            protected override TlsKeyExchange CreateECDHKeyExchange(int keyExchange)
            {
                return new TlsECDHKeyExchange(keyExchange, mSupportedSignatureAlgorithms, mNamedCurves, mClientECPointFormats,mServerECPointFormats);
            }

            public override TlsKeyExchange GetKeyExchange()
            {
                int keyExchangeAlgorithm = TlsUtilities.GetKeyExchangeAlgorithm(mSelectedCipherSuite);

                switch (keyExchangeAlgorithm)
                {
                    case KeyExchangeAlgorithm.DHE_PSK:
                    case KeyExchangeAlgorithm.ECDHE_PSK:
                    case KeyExchangeAlgorithm.PSK:
                    case KeyExchangeAlgorithm.RSA_PSK:
                        return CreatePskKeyExchange(keyExchangeAlgorithm);

                    case KeyExchangeAlgorithm.ECDH_anon:
                    case KeyExchangeAlgorithm.ECDH_ECDSA:
                    case KeyExchangeAlgorithm.ECDH_RSA:
                        return CreateECDHKeyExchange(keyExchangeAlgorithm);

                    default:
                        throw new TlsFatalAlert(AlertDescription.internal_error);
                }
            }
        }
}
