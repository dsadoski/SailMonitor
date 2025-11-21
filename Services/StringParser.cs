namespace SailMonitor.Services
{
    public class StringParser
    {
        public void stringParser()
        {
        }

        public string[] TildaListToStrings(string txt)
        {
            int i;
            string tmp = string.Empty;
            int CNT = 0;
            string[] ARY;

            if (txt.Length == 1 && txt[0] == ' ')
            {
                return new string[0];
            }

            for (i = 0; i < txt.Length; i++)
            {
                if (txt[i] == '~')
                {
                    tmp = string.Empty;
                    CNT++;
                }
                else
                {
                    tmp += txt[i];
                }
            }

            if (tmp.Length > 0)
            {
                CNT++;
            }

            ARY = new string[CNT];
            tmp = string.Empty;
            CNT = 0;

            for (i = 0; i < txt.Length; i++)
            {
                if (txt[i] == '~')
                {
                    ARY[CNT] = tmp;
                    tmp = string.Empty;
                    CNT++;
                }
                else
                {
                    tmp += txt[i];
                }
            }

            if (tmp.Length > 0 && CNT < ARY.Length)
            {
                ARY[CNT] = tmp;
            }

            return ARY;
        }

        public string[] CommaListToString(string txt)
        {
            int i;
            string tmp = string.Empty;
            int CNT = 0;
            string[] ARY;

            if (txt.Length == 1 && txt[0] == ' ')
            {
                return new string[0];
            }

            for (i = 0; i < txt.Length; i++)
            {
                if (txt[i] == ',')
                {
                    tmp = string.Empty;
                    CNT++;
                }
                else
                {
                    tmp += txt[i];
                }
            }

            if (tmp.Length > 0)
            {
                CNT++;
            }

            ARY = new string[CNT];
            tmp = string.Empty;
            CNT = 0;

            for (i = 0; i < txt.Length; i++)
            {
                if (txt[i] == ',')
                {
                    ARY[CNT] = tmp;
                    tmp = string.Empty;
                    CNT++;
                }
                else
                {
                    tmp += txt[i];
                }
            }

            if (tmp.Length > 0 && CNT < ARY.Length)
            {
                ARY[CNT] = tmp;
            }

            return ARY;
        }

        public List<string> CommaListToStringList(string txt)
        {
            int i;
            string tmp = string.Empty;
            int CNT = 0;

            List<string> result = new List<string>();

            if (txt.Length == 1 && txt[0] == ' ')
            {
                return new List<string>();
            }

            for (i = 0; i < txt.Length; i++)
            {
                if (txt[i] == ',')
                {
                    tmp = string.Empty;
                    CNT++;
                }
                else
                {
                    tmp += txt[i];
                }
            }

            if (tmp.Length > 0)
            {
                CNT++;
            }

            tmp = string.Empty;
            CNT = 0;

            for (i = 0; i < txt.Length; i++)
            {
                if (txt[i] == ',')
                {
                    result.Add(tmp);
                    tmp = string.Empty;
                    CNT++;
                }
                else
                {
                    tmp += txt[i];
                }
            }

            if (tmp.Length > 0)
            {
                result.Add(tmp);
            }

            return result;
        }

        public double[] TildaListToDoubles(string txt)
        {
            int i;
            string tmp = string.Empty;
            int CNT = 0;
            double[] ARY;

            if (txt.Length == 1 && txt[0] == ' ')
            {
                return new double[0];
            }

            for (i = 0; i < txt.Length; i++)
            {
                if (txt[i] == '~')
                {
                    tmp = string.Empty;
                    CNT++;
                }
                else
                {
                    tmp += txt[i];
                }
            }

            if (tmp.Length > 0)
            {
                CNT++;
            }

            ARY = new double[CNT];
            tmp = string.Empty;
            CNT = 0;

            for (i = 0; i < txt.Length; i++)
            {
                if (txt[i] == '~')
                {
                    ARY[CNT] = double.Parse(tmp);
                    tmp = string.Empty;
                    CNT++;
                }
                else
                {
                    tmp += txt[i];
                }
            }

            if (tmp.Length > 0 && CNT < ARY.Length)
            {
                ARY[CNT] = double.Parse(tmp);
                tmp = string.Empty;
                CNT++;
            }

            return ARY;
        }

        public int[] TildaListToInts(string txt)
        {
            int i;
            string tmp = string.Empty;
            int count = 0;
            int[] array;

            if (txt.Length == 1 && txt[0] == ' ')
            {
                return new int[0];
            }

            for (i = 0; i < txt.Length; i++)
            {
                if (txt[i] == '~')
                {
                    tmp = string.Empty;
                    count++;
                }
                else
                {
                    tmp += txt[i];
                }
            }

            if (tmp.Length > 0)
            {
                count++;
            }

            array = new int[count];
            tmp = string.Empty;
            count = 0;

            for (i = 0; i < txt.Length; i++)
            {
                if (txt[i] == '~')
                {
                    array[count] = int.Parse(tmp);

                    tmp = string.Empty;
                    count++;
                }
                else
                {
                    tmp += txt[i];
                }
            }

            if (tmp.Length > 0 && count < array.Length)
            {
                array[count] = int.Parse(tmp);
                tmp = string.Empty;
                count++;
            }

            return array;
        }

        public long[] TildaListToLongs(string txt)
        {
            int i;
            string tmp = string.Empty;
            int count = 0;
            long[] ARY;

            if (txt.Length == 1 && txt[0] == ' ')
            {
                return new long[0];
            }

            for (i = 0; i < txt.Length; i++)
            {
                if (txt[i] == '~')
                {
                    tmp = string.Empty;
                    count++;
                }
                else
                {
                    tmp += txt[i];
                }
            }

            if (tmp.Length > 0)
            {
                count++;
            }

            ARY = new long[count];
            tmp = string.Empty;
            count = 0;

            for (i = 0; i < txt.Length; i++)
            {
                if (txt[i] == '~')
                {
                    ARY[count] = long.Parse(tmp);
                    tmp = string.Empty;
                    count++;
                }
                else
                {
                    tmp += txt[i];
                }
            }

            if (tmp.Length > 0 && count < ARY.Length)
            {
                ARY[count] = long.Parse(tmp);
                tmp = string.Empty;
                count++;
            }

            return ARY;
        }
    }
}
