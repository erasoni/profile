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
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;


namespace Ambiesoft
{
    partial class Profile
    {
        static private Mutex createMutex(string filename)
        {
            FileInfo fi = new FileInfo(filename);
            string mutexname = @"Global\profile-" + fi.FullName.ToLower().Replace('\\', '/');
            if (mutexname.Length > 260)
                mutexname = mutexname.Substring(0, 260);

            return new Mutex(false, mutexname);
        }
        static private void waitMutex(Mutex mutex)
        {
            try
            {
                mutex.WaitOne();
            }
            catch (Exception)
            {
                // abandaned causes exception
            }
        }
    }
}
