//MIT License
//
//Copyright (c) 2018 Ambiesoft
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Threading;


namespace Ambiesoft
{
    partial class Profile
    {
        static private string getSectionName(string line)
        {
        	if(string.IsNullOrEmpty(line) || line[0] != '[')
        		return string.Empty;
        		
            StringBuilder sbRet = new StringBuilder();
            for (int i = 1; i < line.Length; ++i)
            {
                Char c = line[i];
                if (c == ']')
                    break;

                sbRet.Append(c);
            }
            return sbRet.ToString();
        }
        static public HashIni ReadAll(String inipath)
        {
            return ReadAll(inipath, false);
        }
        static public HashIni ReadAll(String inipath, bool throwexception)
        {
            Mutex mutex = createMutex(inipath);
            waitMutex(mutex);
            try
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(inipath, System.Text.Encoding.UTF8))
                {
                    return ReadAll(sr);
                }
            }
            catch (Exception e3)
            {
                if (throwexception)
                    throw e3;

                return HashIni.CreateEmptyInstanceForSpecialUse();
            }
            finally
            {
                if (mutex != null)
                {
                    mutex.ReleaseMutex();
                }
            }

        }
        static public HashIni ReadAll(StreamReader sr)
        {
            HashIni hi = HashIni.CreateEmptyInstanceForSpecialUse();
            Hashtable al = hi.Hash;

            {
                
                {

                    String line = null;
                    Hashtable cursec = null;

                    while ((line = sr.ReadLine()) != null)
                    {
                        line = line.TrimStart();
                        if (line.Length == 0 || line[0] == '#')
                            continue;

                        if (line[0] == '[')
                        {
                            String secname = getSectionName(line);
                            cursec = (Hashtable)al[secname];
                            if (cursec == null)
                            {
                                cursec = new Hashtable();
                                al[secname] = cursec;
                            }
                            continue;
                        }
                        else
                        {
                            if (cursec == null)
                                continue;

                            String[] vals = line.Split(new char[] { '=' }, 2);


                            if (vals[0] != null)
                                vals[0] = wpTrim(vals[0]);

                            if (vals.Length < 2)
                            {
                                vals = new String[2] { vals[0], String.Empty };
                            }
                            else
                            {
                                if (vals[1] != null)
                                    vals[1] = wpTrim(vals[1]);
                            }

                            ArrayList arent = (ArrayList)cursec[vals[0]];
                            if (arent == null)
                            {
                                arent = new ArrayList();
                                cursec[vals[0]] = arent;
                            }

                            arent.Add(vals[1]);

                        }


                    }
                }
            }
            return hi;
        }

        static public bool WriteAll(HashIni hi, String inipath)
        {
            return WriteAll(hi, inipath, false);
        }
        static public bool WriteAll(HashIni hi, String inipath, bool throwexception)
        {
            if (hi == null)
                return false;

            Hashtable al = hi.Hash;
            if (al == null)
                return false;

            Mutex mutex = createMutex(inipath);
            waitMutex(mutex);

            try
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(inipath, false, new System.Text.UTF8Encoding(false)))
                {
                    return WriteAll(hi, sw);
                }
            }
            catch (Exception ex)
            {
                if (throwexception)
                    throw ex;

                return false;
            }
            finally
            {
                if (mutex != null)
                {
                    mutex.ReleaseMutex();
                }
            }
        }
        static public bool WriteAll(HashIni hi, System.IO.StreamWriter sw)
        {
            if (hi == null)
                return false;

            Hashtable al = hi.Hash;
            if (al == null)
                return false;

            ArrayList arKeys = new ArrayList(al.Keys);
            arKeys.Sort();

            foreach (String secname in arKeys)
            {
                sw.Write("[");
                sw.Write(secname);
                sw.Write("]");
                sw.WriteLine();

                Hashtable sec = (Hashtable)al[secname];
                if (sec == null)
                    continue;

                ArrayList arKeyKeys = new ArrayList(sec.Keys);
                arKeyKeys.Sort();
                foreach (String keyname in arKeyKeys)
                {
                    ArrayList arent = (ArrayList)sec[keyname];
                    foreach (String val in arent)
                    {
                        if (val != null)
                        {
                            sw.Write(keyname);
                            sw.Write("=");
                            sw.Write(val);
                            sw.WriteLine();
                        }
                    }
                }
                sw.WriteLine();
            }
            return true;
        }

        static public string ReadAllAsString(HashIni hi)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            if (!WriteAll(hi, sw))
                return null;
            sw.Flush();
            string outString = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            sw.Close();
            return outString;
        }
        static public HashIni WriteAllAsString(string iniString)
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(iniString));
            StreamReader sr = new StreamReader(ms);
            return ReadAll(sr);
        }
    }
}
