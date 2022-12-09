using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Zadanie_1IB
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private string KeyText;
        private string KeyText1;
        private string ShifrText;
        private string OpenText;
        private byte[] ByteKey = new byte[8];
        private bool[,] KeyBool = new bool[8,8];
        private int[,] BitKey = new int[8, 8];
        private int[] BitKeyArray = new int[64];
        private byte[] ByteKey1 = new byte[8];
        private int[,] BitKey1 = new int[8, 8];
        private int[] BitKeyArray1 = new int[64];
        private int[] Ci = new int[28];//блоки ключа k0
        private int[] Di = new int[28];
        private int[,] ki = new int[16, 48];//16 раундовых ключей

        //матрица перестановки IP
        private int[] T1 = {
        58,  50,  42,  34,  26,  18,  10,   2,
        60,  52,  44,  36,  28,  20,  12,   4,
        62,  54,  46,  38,  30,  22,  14,   6,
        64,  56,  48,  40,  32,  24,  16,   8,
        57,  49,  41,  33,  25,  17,   9,   1,
        59,  51,  43,  35,  27,  19,  11,   3,
        61,  53,  45,  37,  29,  21,  13,   5,
        63,  55,  47,  39,  31,  23,  15,   7
     };
        //таблица расширения
        private int[] T2= {
        32,   1,   2,   3,   4,   5,
         4,   5,   6,   7,   8,   9,
         8,   9,  10,  11,  12,  13,
        12,  13,  14,  15,  16,  17,
        16,  17,  18,  19,  20,  21,
        20,  21,  22,  23,  24,  25,
        24,  25,  26,  27,  28,  29,
        28,  29,  30,  31,  32,   1
    };
        //s-блок
        private int[] T3 = {
        14,  4, 13,  1,  2, 15, 11,  8,  3, 10,  6, 12,  5,  9,  0,  7,
         0, 15,  7,  4, 14,  2, 13,  1, 10,  6, 12, 11,  9,  5,  3,  8,
         4,  1, 14,  8, 13,  6,  2, 11, 15, 12,  9,  7,  3, 10,  5,  0,
        15, 12,  8,  2,  4,  9,  1,  7,  5, 11,  3, 14, 10,  0,  6, 13,
 
        15,  1,  8, 14,  6, 11,  3,  4,  9,  7,  2, 13, 12,  0,  5, 10,
         3, 13,  4,  7, 15,  2,  8, 14, 12,  0,  1, 10,  6,  9, 11,  5,
         0, 14,  7, 11, 10,  4, 13,  1,  5,  8, 12,  6,  9,  3,  2, 15,
        13,  8, 10,  1,  3, 15,  4,  2, 11,  6,  7, 12,  0,  5, 14,  9,
 
        10,  0,  9, 14,  6,  3, 15,  5,  1, 13, 12,  7, 11,  4,  2,  8,
        13,  7,  0,  9,  3,  4,  6, 10,  2,  8,  5, 14, 12, 11, 15,  1,
        13,  6,  4,  9,  8, 15,  3,  0, 11,  1,  2, 12,  5, 10, 14,  7,
         1, 10, 13,  0,  6,  9,  8,  7,  4, 15, 14,  3, 11,  5,  2, 12,
 
        7, 13, 14,  3,  0,  6,  9, 10,  1,  2,  8,  5, 11, 12,  4, 15,
       13,  8, 11,  5,  6, 15,  0,  3,  4,  7,  2, 12,  1, 10, 14,  9,
       10,  6,  9,  0, 12, 11,  7, 13, 15,  1,  3, 14,  5,  2,  8,  4,
        3, 15,  0,  6, 10,  1, 13,  8,  9,  4,  5, 11, 12,  7,  2, 14,
 
        2, 12,  4,  1,  7, 10, 11,  6,  8,  5,  3, 15, 13,  0, 14,  9,
       14, 11,  2, 12,  4,  7, 13,  1,  5,  0, 15, 10,  3,  9,  8,  6,
        4,  2,  1, 11, 10, 13,  7,  8, 15,  9, 12,  5,  6,  3,  0, 14,
       11,  8, 12,  7,  1, 14,  2, 13,  6, 15,  0,  9, 10,  4,  5,  3,
 
       12,  1, 10, 15,  9,  2,  6,  8,  0, 13,  3,  4, 14,  7,  5, 11,
       10, 15,  4,  2,  7, 12,  9,  5,  6,  1, 13, 14,  0, 11,  3,  8,
        9, 14, 15,  5,  2,  8, 12,  3,  7,  0,  4, 10,  1, 13, 11,  6,
        4,  3,  2, 12,  9,  5, 15, 10, 11, 14,  1,  7,  6,  0,  8, 13,
 
        4, 11,  2, 14, 15,  0,  8, 13,  3, 12,  9,  7,  5, 10,  6,  1,
       13,  0, 11,  7,  4,  9,  1, 10, 14,  3,  5, 12,  2, 15,  8,  6,
        1,  4, 11, 13, 12,  3,  7, 14, 10, 15,  6,  8,  0,  5,  9,  2,
        6, 11, 13,  8,  1,  4, 10,  7,  9,  5,  0, 15, 14,  2,  3, 12,
 
        13,  2,  8,  4,  6, 15, 11,  1, 10,  9,  3, 14,  5,  0, 12,  7,
         1, 15, 13,  8, 10,  3,  7,  4, 12,  5,  6, 11,  0, 14,  9,  2,
         7, 11,  4,  1,  9, 12, 14,  2,  0,  6, 10, 13, 15,  3,  5,  8,
         2,  1, 14,  7,  4, 10,  8, 13, 15, 12,  9,  0,  3,  5,  6, 11
    };
        //P-перестановка
        private int[] T4 = {
        16,   7,  20,  21,
        29,  12,  28,  17,
         1,  15,  23,  26,
         5,  18,  31,  10,
         2,   8,  24,  14,
        32,  27,   3,   9,
        19,  13,  30,   6,
        22,  11,  4,  25
    };
        //Перестановка ключа
        private int[] T5 =  {
        57,  49,  41,  33,  25,  17,   9,
         1,  58,  50,  42,  34,  26,  18,
        10,   2,  59,  51,  43,  35,  27,
        19,  11,   3,  60,  52,  44,  36,
        63,  55,  47,  39,  31,  23,  15,
         7,  62,  54,  46,  38,  30,  22,
        14,   6,  61,  53,  45,  37,  29,
        21,  13,   5,  28,  20,  12,   4,
    };
        //сдвиг ключей 
        private int[] T6 = {
        1,
        1,
        2,
        2,
        2,
        2,
        2,
        2,
        1,
        2,
        2,
        2,
        2,
        2,
        2,
        1,
    };
        //выборка ключей
        private int[] T7 =
        {
        14,   17,   11,   24,   1,   5,
         3,   28,   15,    6,   21,  10,
        23,   19,   12,    4,   26,  8,
        16,    7,   27,   20,   13,  2,
        41,   52,   31,   37,   47,  55,
        30,   40,   51,   45,   33,  48,
        44,   49,   39,   56,   34,  53,
        46,   42,   50,   36,   29,  32
    };
        // Матрица обратной перестановки IP^-1
        private int[] T8 = {
        40,   8,  48,  16,  56,  24,  64,  32,
        39,   7,  47,  15,  55,  23,  63,  31,
        38,   6,  46,  14,  54,  22,  62,  30,
        37,   5,  45,  13,  53,  21,  61,  29,
        36,   4,  44,  12,  52,  20,  60,  28,
        35,   3,  43,  11,  51,  19,  59,  27,
        34,   2,  42,  10,  50,  18,  58,  26,
        33,   1,  41,   9,  49,  17,  57,  25
    };
       
    private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Name = openFileDialog1.FileName;
                textBox1.Clear();
                textBox1.Text = File.ReadAllText(Name);
                
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (saveFileDialog2.ShowDialog() == DialogResult.OK)
            {
                Name = saveFileDialog2.FileName;
                
                File.WriteAllText(Name, textBox2.Text);
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                Name = openFileDialog2.FileName;
                textBox2.Clear();
                textBox2.Text = File.ReadAllText(Name);
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Name = saveFileDialog1.FileName;
                File.WriteAllText(Name, textBox1.Text);
            }
        
    }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            button5.Enabled = false;
            button6.Enabled = false;
            button3.Enabled = true;
            button4.Enabled = true;
            //textBox2.Enabled = false;
           // textBox1.Enabled = true;
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = true;
            button6.Enabled = true;
            //textBox1.Enabled = false;
            //textBox2.Enabled = true;
        }
        private void button7_Click(object sender, EventArgs e)
        {
            if (openFileDialog3.ShowDialog() == DialogResult.OK)
            {
                Name = openFileDialog3.FileName;
                textBox3.Clear();
                textBox3.Text = File.ReadAllText(Name);
                ByteKey = File.ReadAllBytes(Name);
                KeyText = Encoding.Unicode.GetString(ByteKey);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (saveFileDialog3.ShowDialog() == DialogResult.OK)
            {
                Name = saveFileDialog3.FileName;
                File.WriteAllText(Name, textBox3.Text);
            }
        }
        private void button2_Click(object sender, EventArgs e)//генерация ключа
        {
            Random rnd = new Random();
            for (int i = 0; i < 8; i++)
            {
                int res =0;
                for (int k = 0; k < 7; k++)
                {
                    KeyBool[i, k] = rnd.Next(2) == 0 ? false : true;
                    if (KeyBool[i, k] == true) res++;
                }
                if ((res % 2) == 0) KeyBool[i, 7] = false;
                else KeyBool[i, 7] = true;
              
            }           
            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    if(KeyBool[i, k]==false) BitKey[i, k] = 0;
                    else BitKey[i, k] = 1;
                }
            }
            //из битов получаем байты
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0, m = 1; j < 8; j++, m *= 2)
                {
                    if (BitKey[i, 7-j] == 1) ByteKey[i] += (byte)m;
                    if (BitKey[i, 7-j] == 0) ByteKey[i] += (byte)0;
                }
            }
            textBox3.Text = Convert.ToBase64String(ByteKey);
        }
        
        private void firstIP(int [] arr)//начальная перестановка
        {
            int [] buf = new int[64];
            for(int i=0; i<64; i++)
                buf[i] = arr[T1[i]-1];
            for (int i = 0; i < 64; i++)
                arr[i]=buf[i];
        }
        private void endIP(int[] arr)//конечная перестановка
        {
            int[] buf = new int[64];
            for (int i = 0; i < 64; i++)
                buf[i] = arr[T8[i] - 1];
            for (int i = 0; i < 64; i++)
                arr[i] = buf[i];
        }
        private void generate_ki()//генерация ki
        {
            
            for (int i = 0; i < 16; i++)
            {
                int shift = T6[i];
                int[] CiBuf = new int[28];
                int[] DiBuf = new int[28];
                Array.Copy(Ci, shift, CiBuf, 0, 28 - shift);
                Array.Copy(Di, shift, DiBuf, 0, 28 - shift);
                for (int l = 28 - shift, s = 0; l < 28; l++, s++)
                {
                    CiBuf[l] = Ci[s];
                    DiBuf[l] = Di[s];
                }
                for (int j = 0; j < 28; j++)//перезапись Ci и Di
                {
                    Ci[j] = CiBuf[j];
                    Di[j] = DiBuf[j];
                }
                 int[] buf_kl = new int[56];
                for (int j = 0; j < 28; j++)
                {
                    buf_kl[j] = Ci[j];
                    buf_kl[28 + i] = Di[j];
                }
                for (int j = 0; j < 48; j++)
                ki[i,j] = buf_kl[T7[j] - 1];

            }
        }  
        private int[] func(int[] arr, int num)//сложение функции Фейстеля и ключа ki
        {
            int [] Newarr= new int[48];
            for (int j = 0; j < 48; j++)//расширение
                Newarr[j] = arr[T2[j]-1];
            
            int[] res = new int[48];
            for (int j = 0; j < 48; j++)//XOR с ki ключом
            {
                if ((ki[num, j] == 1) && (Newarr[j] == 1)) res[j] = 0;
                if ((ki[num, j] == 0) && (Newarr[j] == 1)) res[j] = 1;
                if ((ki[num, j] == 1) && (Newarr[j] == 0)) res[j] = 1;
                if ((ki[num, j] == 0) && (Newarr[j] == 0)) res[j] = 0;
            }
            int[] f1 = new int[32];
            for (int n = 0; n < 8; n++)//на 8 s-блоков
            {
                int[] s_box = new int[6];
                for (int k = 0; k < 6; k++)//заполнение s-блока
                    s_box[k] = res[n * 6 + k];
                int[] stroka = new int[2];
                // Первый 
                stroka[0] = s_box[0];
                stroka[1] = s_box[5];
                int istroka = 0;
                for (int k = 0; k < 2; k++) // перевод из двоичной в десятичную
                {
                    if (stroka[1 - k ] == 0) continue; // Проверяется каждая цифра с конца и если она равна 0, то ничего не делаем
                    istroka += (int)Math.Pow(2, k); // Все остальные числа возводятся в квадрат и прибавляются в одну переменную
                }
                
                int[] stolbec = new int[4];
                for (int k = 0; k < 4; k++)
                    stolbec[k] = s_box[k + 1];
                int istolbec = 0;
                for (int k = 0; k < 4; k++) // Нужно пройтись по всем цифрам в числе
                {
                    if (stolbec[3 - k] == 0) continue; // Проверяется каждая цифра и если она равна 0, то ничего не делаем, т.к 0 в любой степени равно 0
                    istolbec += (int)Math.Pow(2, k); // Все остальные числа возводятся в квадрат и прибавляются в одну переменную
                }
                int choice = T3[64 * n + istolbec + istroka*16];//получим число в десятичной системе
                string binary = Convert.ToString(choice, 2);//в строку в двочиной системе
                switch (binary.Length)
                {
                    case 1:
                        f1[4 * n] = 0;
                        f1[4 * n+1] = 0;
                        f1[4 * n+2] = 0;
                            if (binary[0] == 49) f1[4 * n + 3] = 1;
                            else f1[4 * n + 3] = 0;
                        break;
                    case 2:
                        f1[4 * n] = 0;
                        f1[4 * n + 1] = 0;
                        for (int k = 2; k < 4; k++)
                        {
                            if (binary[k-2] == 49) f1[4 * n + k] = 1;
                            else f1[4 * n + k] = 0;
                        }
                        break;
                    case 3:
                        f1[4 * n ] = 0;
                        for (int k = 1; k < 4; k++)
                        {
                            if (binary[k-1] == 49) f1[4 * n + k] = 1;
                            else f1[4 * n + k] = 0;
                        }
                        break;
                    case 4:
                        for (int k = 0; k < 4; k++)
                        {
                            if (binary[k] == 49) f1[4 * n + k] = 1;
                            else f1[4 * n + k] = 0;
                        }
                        break;
                    default:
                        
                        break;
                }

            }
            int[] resP = new int[32];
            for (int k = 0; k < 32; k++)
                resP[k]=f1[T4[k]-1] ;
            return resP;

        }
        private void feistel(int[] left, int[] right)//перестановка Фейстеля
        { 
            for (int i=0; i<16; i++)
            {
                int[] copy_right = new int[32];
                for (int k = 0; k < 32; k++)
                    copy_right[k] = right[k];
                int[] f = new int[32];
                //int flag = 1;//флаг режима шифрования
                f=func(right, i);
                for (int j = 0; j < 32; j++)//XOR с левой частью и запись в правую
                {
                    if ((f[j] == 1) && (left[j] == 1)) right[j] = 0;
                    if ((f[j] == 0) && (left[j] == 1)) right[j] = 1;
                    if ((f[j] == 1) && (left[j] == 0)) right[j] = 1;
                    if ((f[j] == 0) && (left[j] == 0)) right[j] = 0;
                }
                for (int k = 0; k < 32; k++)//запись правой части в левую
                    left[k] = copy_right[k];
            }
        }
        private void back_feistel(int[] left, int[] right)//обратная перестановка Фейстеля
        {
            for (int i = 0; i < 16; i++)
            {
                int[] copy_left = new int[32];
                for (int k = 0; k < 32; k++)
                    copy_left[k] = left[k];
                int[] f = new int[32];
                //int flag = -1;//флаг режима дешифрования
                f = func(left, 15-i);//15-i чтобы использовать ключи в обратном порядке
                for (int j = 0; j < 32; j++)//XOR с правой частью и запись в левую
                {
                    if (f[j] == 1 && right[j] == 1) left[j] = 0;
                    if (f[j] == 0 && right[j] == 1) left[j] = 1;
                    if (f[j] == 1 && right[j] == 0) left[j] = 1;
                    if (f[j] == 0 && right[j] == 0) left[j] = 0;
                }
                for (int k = 0; k < 32; k++)//запись левой части в праву
                    right[k] = copy_left[k];
            }
        }  
        public void button1_Click(object sender, EventArgs e)
        {
            if (radioButton2.Checked)//шифрование
            {
                //считываем текст 
                //OpenText = textBox1.Text;
                byte[] strBytes = Encoding.Unicode.GetBytes(textBox1.Text);
                int[,] Bit_text = new int[strBytes.Length, 8];
                //получаем биты текста
                for (int i = 0; i < strBytes.Length; i++)
                {
                    for (int j = 0; j < 8; j++)
                        Bit_text[i, 7-j] = (strBytes[i] >> j) & 0x01;// заполнение начиная с конца байта!!!!

                }
                int[] BitsText = new int[strBytes.Length * 8];
                int z = 0;
                for (int i = 0; i < strBytes.Length; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        BitsText[z] = Bit_text[i, 7-j];
                        z++;
                    }

                //    ключ
                if (textBox3.Text != "")
                {
                    //KeyText = textBox3.Text;
                    ByteKey = Convert.FromBase64String(textBox3.Text);
                    for (int i = 0; i < 8; i++)
                        for (int j = 0; j < 8; j++)
                            BitKey[i, 7 - j] = (ByteKey[i] >> j) & 0x01;
                }
                else
                {
                    MessageBox.Show("Используется ключ по умолчанию.", "Внимание!");
                    KeyText = "1234";
                }
                int r = 0;
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        BitKeyArray[r] = BitKey[i, 7 - j];
                        r++;
                    }
                //делим текст на блоки по 64 бита
                int adds = BitsText.Length % 64;//лишние биты
                int N = 0;//размер массива шифрованных битов
                if (adds == 0)
                 N = BitsText.Length; 
                else
                 N = BitsText.Length + 64 - adds;

                int[] BitsTextAll = new int[N];
                for (int k = 0; k < BitsText.Length; k++)
                    BitsTextAll[k] = BitsText[k];
                if (adds != 0)
                    for (int k = BitsText.Length ; k < N; k++)//для битов последнего блока 
                    {
                        BitsTextAll[k] = 0;//заполняем нулями конец 
                    }

                int[] ShifrBitsText = new int[N];
                for (int j = 0; j < BitsTextAll.Length/ 64; j++)//от 0 до кол-ва блоков
                {
                    for (int i = 0; i < 28; i++)//на 2 блока с перестановкой(нач подготовка ключа)
                    {
                        Ci[i] = BitKeyArray[T5[i] - 1];
                        Di[i] = BitKeyArray[T5[28 + i] - 1];
                    }
                    generate_ki();//гшенерация ki
                    int[] BufTextBits = new int[64];
                    for (int k = 0; k < 64; k++)//для каждого бита в блоке
                        BufTextBits[k] = BitsTextAll[64 * j + k];
                    firstIP(BufTextBits);//начальная престановка
                    int[] LeftBuf = new int[32];
                    int[] RightBuf = new int[32];
                    for (int i = 0; i < 32; i++)//на левую и правую часть
                    {
                        LeftBuf[i] = BufTextBits[i];
                        RightBuf[i] = BufTextBits[32 + i];
                    }
                    feistel(LeftBuf, RightBuf);//преобразование Фейстеля
                    for (int k = 0; k < 32; k++)//для каждого бита в блоке
                    {
                        BufTextBits[k] = LeftBuf[k];
                        BufTextBits[32 + k] = RightBuf[k];
                    }
                    endIP(BufTextBits);
                    for (int k = 0; k < 64; k++)//для каждого бита в блоке
                        ShifrBitsText[64 * j + k] = BufTextBits[k];

                }

                byte[] ShifrBytesText = new byte[ShifrBitsText.Length / 8];
                //из битов получаем байты
                for (int i = 0; i < ShifrBitsText.Length / 8; i++)//кол-во символов
                    for (int j = 0, m = 1; j < 8; j++, m *= 2)
                    {
                        if (ShifrBitsText[i * 8 +  j] == 1) ShifrBytesText[i] += (byte)m;
                        if (ShifrBitsText[i * 8 +  j] == 0) ShifrBytesText[i] += (byte)0;
                    }
               
                textBox2.Text = Convert.ToBase64String(ShifrBytesText);
                //ShifrText = Encoding.Unicode.GetString(ShifrBytesText);
                //for (int j = 0; j < 8; j++)//для каждого бита в блоке
                //    test[j] = ShifrBytesText[j];

            }

            if (radioButton1.Checked)//дешифровка
            {
                textBox1.Clear();
                //ShifrText = textBox2.Text;
                byte[] strBytes = Convert.FromBase64String(textBox2.Text);

                int[,] Bit_text = new int[strBytes.Length, 8];

                //получаем биты текста
                for (int i = 0; i < strBytes.Length; i++)
                    for (int j = 0; j < 8; j++)
                        Bit_text[i, 7 - j] = (strBytes[i] >> j) & 0x01;// заполнение начиная с конца байта!!!!
                int[] BitsText = new int[strBytes.Length * 8];
                int z = 0;
                for (int i = 0; i < strBytes.Length; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        BitsText[z] = Bit_text[i, 7 - j];
                        z++;
                    }

                //ключ
               
                if (textBox3.Text != "")
                {
                    //KeyText1 = textBox3.Text;
                    ByteKey1 = Convert.FromBase64String(textBox3.Text);
                    for (int i = 0; i < 8; i++)
                        for (int j = 0; j < 8; j++)
                            BitKey1[i, 7 - j] = (ByteKey1[i] >> j) & 0x01;
                }
                else
                {
                    MessageBox.Show("Используется ключ по умолчанию.", "Внимание!");
                    KeyText1 = "1234";
                }
                int r = 0;
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        BitKeyArray1[r] = BitKey1[i, 7 - j];
                        r++;
                    }
                //делим текст на блоки по 64 бита
                int N = BitsText.Length;
                int[] OpenBitsText = new int[N];
                for (int j = 0; j < (BitsText.Length / 64); j++)//от 0 до кол-ва блоков
                {
                    for (int i = 0; i < 28; i++)//на 2 блока с перестановкой 
                    {
                        Ci[i] = BitKeyArray1[T5[i] - 1];
                        Di[i] = BitKeyArray1[T5[28 + i] - 1];
                    }
                    generate_ki();//генерация ki
                    int[] BufTextBits = new int[64];
                    for (int k = 0; k < 64; k++)//для каждого бита в блоке
                        BufTextBits[k] = BitsText[64 * j + k];
                    firstIP(BufTextBits);
                    int[] LeftBuf = new int[32];
                    int[] RightBuf = new int[32];
                    for (int i = 0; i < 32; i++)//на левую и правую часть
                    {
                        LeftBuf[i] = BufTextBits[i];
                        RightBuf[i] = BufTextBits[32 + i];
                    }
                    back_feistel(LeftBuf, RightBuf);//обратное преобразование Фейстеля???
                    for (int k = 0; k < 32; k++)//для каждого бита в блоке??
                    {
                        BufTextBits[k] = LeftBuf[k];
                        BufTextBits[32 + k] = RightBuf[k];
                    }
                   endIP(BufTextBits);
                    for (int k = 0; k < 64; k++)//для каждого бита в блоке
                        OpenBitsText[64 * j + k] = BufTextBits[k];
                }
                byte[] OpenrBytesText = new byte[OpenBitsText.Length / 8];
                //из битов получаем байты
                for (int i = 0; i < OpenBitsText.Length / 8; i++)//кол-во символов
                    for (int j = 0, m = 1; j < 8; j++, m *= 2)
                    {
                        if (OpenBitsText[i * 8 + j] == 1) OpenrBytesText[i] += (byte)m;
                        if (OpenBitsText[i * 8 + j] == 0) OpenrBytesText[i] += (byte)0;
                    }
               textBox1.Text = Encoding.Unicode.GetString(OpenrBytesText);
            }
        }
 
    }
}
