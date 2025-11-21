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
            int count = 0;
            string[] array;

            if (txt.Length == 1 && txt[0] == ' ')
            {
                return new string[0];
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

            array = new string[count];
            tmp = string.Empty;
            count = 0;

            for (i = 0; i < txt.Length; i++)
            {
                if (txt[i] == '~')
                {
                    array[count] = tmp;
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
                array[count] = tmp;
            }

            return array;
        }

        public string[] CommaListToString(string txt)
        {
            int i;
            string tmp = string.Empty;
            int count = 0;
            string[] array;

            if (txt.Length == 1 && txt[0] == ' ')
            {
                return new string[0];
            }

            for (i = 0; i < txt.Length; i++)
            {
                if (txt[i] == ',')
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

            array = new string[count];
            tmp = string.Empty;
            count = 0;

            for (i = 0; i < txt.Length; i++)
            {
                if (txt[i] == ',')
                {
                    array[count] = tmp;
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
                array[count] = tmp;
            }

            return array;
        }

        public List<string> CommaListToStringList(string txt)
        {
            int i;
            string tmp = string.Empty;
            int count = 0;

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

            tmp = string.Empty;
            count = 0;

            for (i = 0; i < txt.Length; i++)
            {
                if (txt[i] == ',')
                {
                    result.Add(tmp);
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
                result.Add(tmp);
            }

            return result;
        }

        public double[] TildaListToDoubles(string txt)
        {
            int i;
            string tmp = string.Empty;
            int count = 0;
            double[] array;

            if (txt.Length == 1 && txt[0] == ' ')
            {
                return new double[0];
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

            array = new double[count];
            tmp = string.Empty;
            count = 0;

            for (i = 0; i < txt.Length; i++)
            {
                if (txt[i] == '~')
                {
                    array[count] = double.Parse(tmp);
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
                array[count] = double.Parse(tmp);
                tmp = string.Empty;
                count++;
            }

            return array;
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
            long[] array;

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

            array = new long[count];
            tmp = string.Empty;
            count = 0;

            for (i = 0; i < txt.Length; i++)
            {
                if (txt[i] == '~')
                {
                    array[count] = long.Parse(tmp);
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
                array[count] = long.Parse(tmp);
                tmp = string.Empty;
                count++;
            }

            return array;
        }
    }
}
