using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CryptoApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            ResultTextBox.Clear();

            var saferTask = Task.Run(() => SAFER_Encrypt("Hello SAFER"));
            var md2Task = Task.Run(() => MD2_Hash("Hello MD2"));
            var plesTask = Task.Run(() => PlesseyGenerate());

            var results = await Task.WhenAll(saferTask, md2Task, plesTask);

            foreach (var result in results)
            {
                ResultTextBox.AppendText(result + Environment.NewLine);
            }
        }

        private string SAFER_Encrypt(string input)
        {
            // Dummy implementation for SAFER encryption
            // Replace with actual implementation if available
            return $"SAFER Encrypted: {input}";
        }

        private string MD2_Hash(string input)
        {
            using (var md2 = new MD2Managed())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md2.ComputeHash(inputBytes);
                return $"MD2 Hash: {BitConverter.ToString(hashBytes).Replace("-", "")}";
            }
        }

        private string PlesseyGenerate()
        {
            // Dummy implementation for Plessey random number generation
            // Replace with actual implementation if available
            return $"Plessey Generated: {new Random().Next()}";
        }
    }

    public class MD2Managed : HashAlgorithm
    {
        private byte[] state;
        private byte[] checksum;
        private byte[] buffer;
        private int count;

        public MD2Managed()
        {
            Initialize();
        }

        public override void Initialize()
        {
            state = new byte[16];
            checksum = new byte[16];
            buffer = new byte[16];
            count = 0;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            for (int i = 0; i < cbSize; i++)
            {
                buffer[count++] = array[ibStart + i];
                if (count == 16)
                {
                    TransformBlock(buffer, 0);
                    count = 0;
                }
            }
        }

        protected override byte[] HashFinal()
        {
            byte[] padding = new byte[16 - count];
            for (int i = 0; i < padding.Length; i++)
            {
                padding[i] = (byte)(16 - count);
            }

            HashCore(padding, 0, padding.Length);
            HashCore(checksum, 0, checksum.Length);

            return state;
        }

        private void TransformBlock(byte[] block, int offset)
        {
            byte[] x = new byte[48];

            for (int i = 0; i < 16; i++)
            {
                x[i] = state[i];
                x[i + 16] = block[offset + i];
                x[i + 32] = (byte)(state[i] ^ block[offset + i]);
            }

            for (int i = 0; i < 18; i++)
            {
                for (int j = 0; j < 48; j++)
                {
                    x[j] ^= S[i];
                }
                Array.Copy(x, 1, x, 0, 47);
                x[47] ^= S[i];
            }

            Array.Copy(x, 0, state, 0, 16);

            for (int i = 0; i < 16; i++)
            {
                checksum[i] ^= block[offset + i];
            }
        }

        private static readonly byte[] S = new byte[]
        {
            0x29, 0x2e, 0x43, 0x39, 0x7b, 0xc9, 0x03, 0x86, 0xd4, 0x4f, 0x4a, 0xc0, 0x1e, 0x83, 0x13, 0x8b,
            0x0b, 0x4d, 0x03, 0x18, 0x0e, 0x8b, 0x39, 0x22, 0x85, 0x09, 0x6a, 0x31, 0x05, 0x2e, 0x91, 0xc5,
            0x9c, 0x11, 0x5d, 0xe3, 0x3f, 0x07, 0x8d, 0xd0, 0xd2, 0x98, 0xbb, 0x16, 0x66, 0x5a, 0xe2, 0x06,
            0x84, 0x0d, 0xb4, 0x54, 0xd6, 0x4f, 0x52, 0x04, 0x29, 0x19, 0xf9, 0xd0, 0x38, 0xe0, 0xb3, 0xd5,
            0x84, 0x0d, 0xb4, 0x54, 0xd6, 0x4f, 0x52, 0x04, 0x29, 0x19, 0xf9, 0xd0, 0x38, 0xe0, 0xb3, 0xd5
        };
    }
}
