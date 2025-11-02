using SimpleLogManager.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLogManager
{
    public class ByteConversion
    {
        readonly static List<ByteSize> sizes = [
            ByteSize.Byte,
            ByteSize.KiloByte,
            ByteSize.MegaByte,
            ByteSize.GigaByte,
            ByteSize.PetaByte
        ];

        public static int GetIndex(ByteSize size)
        {
            for (int i = 0; i < sizes.Count; i++)
            {
                if (sizes[i] == size) return i;
            }

            return -1;
        }

        public static ByteSize? StringToByteSize(string? str)
        {
            if (str is null) return null;

            switch (str.ToUpper())
            {
                case "B":
                case "BYTE":
                    return ByteSize.Byte;
                case "KB":
                case "KILOBYTE":
                    return ByteSize.KiloByte;
                case "MB":
                case "MEGABYTE":
                    return ByteSize.MegaByte;
                case "GB":
                case "GIGABYTE":
                    return ByteSize.GigaByte;
                case "PB":
                case "PETABYTE":
                    return ByteSize.PetaByte;
            }

            return null;
        }

        public static double Convert(double amount, ByteSize from, ByteSize to, ByteSizeType byteSizeType = ByteSizeType.Binary)
        {
            (int f, int t) = (GetIndex(from), GetIndex(to));

            int i = to - from;

            int baseUnit = byteSizeType is ByteSizeType.Decimal ? 1000 : 1024;

            while (i > 0)
            {
                amount /= baseUnit;
                i--;
            }

            while (i < 0)
            {
                amount *= baseUnit;
                i++;
            }

            return amount;
        }
    }
}
