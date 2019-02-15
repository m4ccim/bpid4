using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace bpid4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }
        private const int sizeOfBlock = 64;
        private const int shiftKey = 2; 

        private const int quantityOfRounds = 16; 

        private string BinaryToRightLength(string input)
        {
            while ((input.Length % sizeOfBlock) != 0)
                input += '0';

            return input;
        }

        string[] Blocks; 



        private void buttonEncrypt_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"D:\Desktop\",
                Title = "Browse Files",

                CheckFileExists = true,
                CheckPathExists = true,

                Filter = "All files (*)|*",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                byte[] fileBytes = File.ReadAllBytes(openFileDialog1.FileName);
                StringBuilder sb = new StringBuilder();

                foreach (byte b in fileBytes)
                {
                    sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
                }
                label1.Text = openFileDialog1.FileName;
                CutBinaryStringIntoBlocks(sb.ToString());


                string key = "nvidia";

                key = StringToBinaryFormat(key);

                for (int j = 0; j < quantityOfRounds; j++)
                {
                    for (int i = 0; i < Blocks.Length; i++)
                        Blocks[i] = EncodeDES_One_Round(Blocks[i], key);

                    key = KeyToNextRound(key);
                }

                richTextBox1.Text += (Blocks[Blocks.Length - 2].Substring(0, Convert.ToInt32(comboBox1.SelectedItem)) + "\n");
            }
        }

        private string StringToBinaryFormat(string input)
        {
            string output = "";

            for (int i = 0; i < input.Length; i++)
            {
                string char_binary = Convert.ToString(input[i], 2);

                while (char_binary.Length < 8)
                    char_binary = "0" + char_binary;

                output += char_binary;
            }

            return output;
        }


        private void CutBinaryStringIntoBlocks(string input)
        {
            Blocks = new string[input.Length / sizeOfBlock];

            int lengthOfBlock = input.Length / Blocks.Length;

            for (int i = 0; i < Blocks.Length; i++)
                Blocks[i] = input.Substring(i * lengthOfBlock, lengthOfBlock);
        }


        private string EncodeDES_One_Round(string input, string key)
        {
            string L = input.Substring(0, input.Length / 2);
            string R = input.Substring(input.Length / 2, input.Length / 2);

            return (R + XOR(L, f(R, key)));
        }

        private string XOR(string s1, string s2)
        {
            string result = "";

            for (int i = 0; i < s1.Length; i++)
            {
                bool a = Convert.ToBoolean(Convert.ToInt32(s1[i].ToString()));
                bool b = Convert.ToBoolean(Convert.ToInt32(s2[i].ToString()));

                if (a ^ b)
                    result += "1";
                else
                    result += "0";
            }
            return result;
        }

        private string f(string s1, string s2)
        {
            return XOR(s1, s2);
        }


        private string KeyToNextRound(string key)
        {
            for (int i = 0; i < shiftKey; i++)
            {
                key = key[key.Length - 1] + key;
                key = key.Remove(key.Length - 1);
            }

            return key;
        }


    }
}
