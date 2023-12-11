using System.Collections;

namespace lib.Labs.Encryptors;

public class DesAlgorithm
{
    private readonly BitArray[,] S_Box1 = new BitArray[4, 4];
    private readonly BitArray[,] S_Box2 = new BitArray[4, 4];
    private readonly BitArray Master_key; //10-битный ключ

    public DesAlgorithm(string key)
    {
        Master_key = new BitArray(10);
        for (int i = 0; i < key.Length; i++)
        {
            Master_key[i] = StringToBinary(key[i]);
        }

        var b0 = new BitArray(2)
        {
            [0] = false,
            [1] = false
        };

        var b1 = new BitArray(2)
        {
            [0] = false,
            [1] = true
        };

        var b2 = new BitArray(2)
        {
            [0] = true,
            [1] = false
        };

        var b3 = new BitArray(2)
        {
            [0] = true,
            [1] = true
        };

        S_Box1[0, 0] = b1;
        S_Box1[0, 1] = b0;
        S_Box1[0, 2] = b3;
        S_Box1[0, 3] = b2;

        S_Box1[1, 0] = b3;
        S_Box1[1, 1] = b2;
        S_Box1[1, 2] = b1;
        S_Box1[1, 3] = b0;

        S_Box1[2, 0] = b0;
        S_Box1[2, 1] = b2;
        S_Box1[2, 2] = b1;
        S_Box1[2, 3] = b3;

        S_Box1[3, 0] = b3;
        S_Box1[3, 1] = b1;
        S_Box1[3, 2] = b3;
        S_Box1[3, 3] = b2;

        S_Box2[0, 0] = b0;
        S_Box2[0, 1] = b1;
        S_Box2[0, 2] = b2;
        S_Box2[0, 3] = b3;

        S_Box2[1, 0] = b2;
        S_Box2[1, 1] = b0;
        S_Box2[1, 2] = b1;
        S_Box2[1, 3] = b3;

        S_Box2[2, 0] = b3;
        S_Box2[2, 1] = b0;
        S_Box2[2, 2] = b1;
        S_Box2[2, 3] = b0;

        S_Box2[3, 0] = b2;
        S_Box2[3, 1] = b1;
        S_Box2[3, 2] = b0;
        S_Box2[3, 3] = b3;
    }

    public byte Encrypt(byte block)
    {
        var bits_block = ByteToBits(block);
        var keys = GenerateKeys();
        return BitsToByte(RIP(Fk(Switch(Fk(IP(bits_block), keys[0])), keys[1])));
    }

    public byte Decrypt(byte block)
    {
        var bits_block = ByteToBits(block);
        var keys = GenerateKeys();
        return BitsToByte(RIP(Fk(Switch(Fk(IP(bits_block), keys[1])), keys[0])));
    }

    /// <summary>
    /// Генерирует два ключа из исходного ключа с использованием операций перестановки и циклического сдвига.
    /// </summary>
    /// <returns></returns>
    private BitArray[] GenerateKeys()
    {
        var keys = new BitArray[2];
        var temp = SplitBlock(P10(Master_key));
        keys[0] = P8(CircularLeftShift(temp[0], 1), CircularLeftShift(temp[1], 1));
        keys[1] = P8(CircularLeftShift(temp[0], 3), CircularLeftShift(temp[1], 3)); //1 + 2 = 3
        return keys;
    }

    /// <summary>
    /// Генерация перестановочного массива P10
    /// </summary>
    /// <param name="key">ключ</param>
    /// <returns></returns>
    private BitArray P10(BitArray key)
    {
        //0 1 2 3 4 5 6 7 8 9
        //2 4 1 6 3 9 0 8 7 5
        var permutatedArray = new BitArray(10);

        permutatedArray[0] = key[2];
        permutatedArray[1] = key[4];
        permutatedArray[2] = key[1];
        permutatedArray[3] = key[6];
        permutatedArray[4] = key[3];
        permutatedArray[5] = key[9];
        permutatedArray[6] = key[0];
        permutatedArray[7] = key[8];
        permutatedArray[8] = key[7];
        permutatedArray[9] = key[5];

        return permutatedArray;
    }

    /// <summary>
    /// Генерация пересестановочного массива P8
    /// </summary>
    /// <param name="part1">1 часть ключа</param>
    /// <param name="part2">2 часть ключа</param>
    /// <returns></returns>
    private BitArray P8(BitArray part1, BitArray part2)
    {
        //0 1 2 3 4 5 6 7
        //5 2 6 3 7 4 9 8
        //6 3 7 4 8 5 10 9
        var permutatedArray = new BitArray(8);

        permutatedArray[0] = part2[0]; //5
        permutatedArray[1] = part1[2];
        permutatedArray[2] = part2[1]; //6
        permutatedArray[3] = part1[3];
        permutatedArray[4] = part2[2]; //7
        permutatedArray[5] = part1[4];
        permutatedArray[6] = part2[4]; //9
        permutatedArray[7] = part2[3]; //8

        return permutatedArray;
    }

    /// <summary>
    /// Генерация пересестановочного массива P4
    /// </summary>
    /// <param name="part1">1 часть ключа</param>
    /// <param name="part2">2 часть ключа</param>
    /// <returns></returns>
    private BitArray P4(BitArray part1, BitArray part2)
    {
        //0 1 2 3
        //2 4 3 1
        //1 3 2 0
        var permutatedArray = new BitArray(4);

        permutatedArray[0] = part1[1];
        permutatedArray[1] = part2[1]; //3
        permutatedArray[2] = part2[0]; //2
        permutatedArray[3] = part1[0];

        return permutatedArray;
    }

    /// <summary>
    /// Генерация расширенного массива
    /// </summary>
    /// <param name="input">входной массив</param>
    /// <returns></returns>
    private BitArray EP(BitArray input)
    {
        //0 1 2 3
        //4 1 2 3 2 3 4 1
        //3 0 1 2 1 2 3 0
        var permutatedArray = new BitArray(8);

        permutatedArray[0] = input[3];
        permutatedArray[1] = input[0];
        permutatedArray[2] = input[1];
        permutatedArray[3] = input[2];
        permutatedArray[4] = input[1];
        permutatedArray[5] = input[2];
        permutatedArray[6] = input[3];
        permutatedArray[7] = input[0];

        return permutatedArray;
    }

    /// <summary>
    /// Генерация перестанованного массива 
    /// </summary>
    /// <param name="plainText">входной текст</param>
    /// <returns></returns>
    private BitArray IP(BitArray plainText)
    {
        //0 1 2 3 4 5 6 7
        //1 5 2 0 3 7 4 6
        var permutatedArray = new BitArray(8);

        permutatedArray[0] = plainText[1];
        permutatedArray[1] = plainText[5];
        permutatedArray[2] = plainText[2];
        permutatedArray[3] = plainText[0];
        permutatedArray[4] = plainText[3];
        permutatedArray[5] = plainText[7];
        permutatedArray[6] = plainText[4];
        permutatedArray[7] = plainText[6];

        return permutatedArray;
    }

    /// <summary>
    /// Выполняет обратную операцию к IP и используется в конце процесса расшифрования.
    /// </summary>
    /// <param name="permutedText">перестановочный текст</param>
    /// <returns></returns>
    private BitArray RIP(BitArray permutedText)
    {
        //0 1 2 3 4 5 6 7 
        //3 0 2 4 6 1 7 5

        var permutatedArray = new BitArray(8);

        permutatedArray[0] = permutedText[3];
        permutatedArray[1] = permutedText[0];
        permutatedArray[2] = permutedText[2];
        permutatedArray[3] = permutedText[4];
        permutatedArray[4] = permutedText[6];
        permutatedArray[5] = permutedText[1];
        permutatedArray[6] = permutedText[7];
        permutatedArray[7] = permutedText[5];

        return permutatedArray;
    }

    /// <summary>
    /// Реализует циклический сдвиг влево для массива 'a' на 'bitNumber' битов.
    /// </summary>
    /// <param name="a">массив</param>
    /// <param name="bitNumber">кол-во бит</param>
    /// <returns></returns>
    private BitArray CircularLeftShift(BitArray a, int bitNumber)
    {
        var shifted = new BitArray(a.Length);
        var index = 0;
        for (int i = bitNumber; index < a.Length; i++)
        {
            shifted[index++] = a[i % a.Length];
        }

        return shifted;
    }

    /// <summary>
    /// Разделяет блок на две части
    /// </summary>
    /// <param name="block">блок</param>
    /// <returns></returns>
    private BitArray[] SplitBlock(BitArray block)
    {
        var splited = new BitArray[2];
        splited[0] = new BitArray(block.Length / 2);
        splited[1] = new BitArray(block.Length / 2);
        var index = 0;

        for (int i = 0; i < block.Length / 2; i++)
        {
            splited[0][i] = block[i];
        }

        for (int i = block.Length / 2; i < block.Length; i++)
        {
            splited[1][index++] = block[i];
        }

        return splited;
    }

    /// <summary>
    /// Выполняет операцию замены для блока с использованием S-боксов.
    /// </summary>
    /// <param name="input">входной блок</param>
    /// <param name="num">номер s-box</param>
    /// <returns></returns>
    private BitArray SBoxes(BitArray input, int num)
    {
        BitArray[,] current_S_Box;

        current_S_Box = num == 1 ? S_Box1 : S_Box2;

        return current_S_Box[BinaryStrToDecimal(BinaryToString(input[0]) + BinaryToString(input[3])),
            BinaryStrToDecimal(BinaryToString(input[1]) + BinaryToString(input[2]))];
    }

    /// <summary>
    /// Реализует функцию F для DES, которая включает в себя расширение блока,
    /// операцию XOR с подключом sk, деление на две части, замену S-боксов, и окончательную перестановку.
    /// </summary>
    /// <param name="right">блок</param>
    /// <param name="sk">подключ</param>
    /// <returns></returns>
    private BitArray F(BitArray right, BitArray sk)
    {
        var temp = SplitBlock(EP(right).Xor(sk));
        return P4(SBoxes(temp[0], 1), SBoxes(temp[1], 2));
    }

    /// <summary>
    /// Применяет функцию F для одного раунда шифрования.
    /// Принимает входной блок IP и ключ раунда key.
    /// Выполняет необходимые операции для обработки левой и правой частей блока, а затем объединяет их.
    /// </summary>
    /// <param name="IP"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    private BitArray Fk(BitArray IP, BitArray key)
    {
        var temp = SplitBlock(IP);
        var Left = temp[0].Xor(F(temp[1], key));
        var joined = new BitArray(8);
        var index = 0;
        for (int i = 0; i < 4; i++)
        {
            joined[index++] = Left[i];
        }

        for (int i = 0; i < 4; i++)
        {
            joined[index++] = temp[1][i];
        }

        return joined;
    }

    /// <summary>
    /// Реализует операцию переключения. Меняет местами левую и правую части восьмибитного блока.
    /// </summary>
    /// <param name="input">8-бит блок</param>
    /// <returns></returns>
    private BitArray Switch(BitArray input)
    {
        var switched = new BitArray(8);
        var index = 0;
        for (int i = 4; index < input.Length; i++)
        {
            switched[index++] = input[i % input.Length];
        }

        return switched;
    }

    // decimal to binary string
    private string DecimalToBinaryString(byte num)
    {
        var ret = "";
        for (int i = 0; i < 8; i++)
        {
            if (num % 2 == 1)
                ret = "1" + ret;
            else
                ret = "0" + ret;
            num >>= 1;
        }

        return ret;
    }

    // binary to decimal string
    private byte BinaryStrToDecimal(string binstr)
    {
        byte ret = 0;
        foreach (var t in binstr)
        {
            ret <<= 1;
            if (t == '1')
                ret++;
        }

        return ret;
    }

    private string BinaryToString(bool input)
    {
        return input ? "1" : "0";
    }

    private bool StringToBinary(char bit)
    {
        return bit switch
        {
            '0' => false,
            '1' => true,
            _ => throw new Exception("Key should be in binary format [0,1]")
        };
    }

    private BitArray ByteToBits(byte block)
    {
        var bits = DecimalToBinaryString(block);
        var result = new BitArray(8);
        for (int i = 0; i < bits.Length; i++)
        {
            result[i] = StringToBinary(bits[i]);
        }

        return result;
    }

    private byte BitsToByte(BitArray block)
    {
        var result = "";
        for (int i = 0; i < block.Length; i++)
        {
            result += BinaryToString(block[i]);
        }

        return BinaryStrToDecimal(result);
    }
}