using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace Sort_Image_by_Date_Taked
{
    class Program
    {
        static public Queue<string> jpgPathQuery = new Queue<string>();
        static public int count;
        static public string DestinationPath;
        static public string date, time;

        static int Main(string[] args)
        {
            //Just make a queue to load all files in it
            DestinationPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + Path.DirectorySeparatorChar + "export"; 
            if (args.Length > 0)
            {
                string path = args[0];
                if (String.Empty != path)
                {
                    try
                    {
                        FileAttributes attr = File.GetAttributes(path);
                        if (attr.HasFlag(FileAttributes.Directory)) {
                            Console.WriteLine("Path is a Directory: " + path);
                            DirSearch(path);
                            Console.WriteLine("Total enqueued: " + count.ToString());

                            int countx = jpgPathQuery.Count;
                            for(int i =0; i < countx; i++)
                            {
                                StartSortingJpegs();
                            }

                        }
                        else
                        {
                            Console.WriteLine("Path is a File: " + path);
                        }
                    }
                    catch(FileNotFoundException e)
                    {
                        Console.WriteLine("No file found: " + e.Message);
                    }
                }
            }
            else
            {
                Console.WriteLine("Pass director or jpg image path first.");
                Console.WriteLine("Program Close");
            }
            Console.ReadLine();
            return 0;
        }


        static void StartSortingJpegs()
        {
            string image = jpgPathQuery.Dequeue();
            string ext = Path.GetExtension(image).ToLower();
            using (Bitmap b = new Bitmap(image))
            {
                PropertyItem[] propItems = b.PropertyItems;

                ASCIIEncoding encodings = new ASCIIEncoding();

                foreach (PropertyItem item in propItems)
                {
                    if(item.Id == 0x9003)
                    {
                    Console.WriteLine("0x" + item.Id.ToString("x"));
                    try
                    {
                        string DT = encodings.GetString(item.Value);
                        Console.WriteLine("The Date & Time is " + DT.Replace(":","-"));


                            string[] tmp = DT.Replace(":", "-").Split(' ');
                            date = tmp[0];
                            time = tmp[1].Substring(0,tmp[1].Length - 1);

                            //Move(image, DestinationPath, date, time, ext);

                    }
                    catch(EncoderFallbackException e)
                    {
                        Console.WriteLine("No Date Time");
                    }
                    Console.WriteLine("");
                    }
                   
                }
            }

            Move(image, DestinationPath, date, time, ext);
        }
        static void Move(string srcPath, string destPath, string dir, string fileName, string ext)
        {
            string path = DestinationPath + Path.DirectorySeparatorChar + dir;
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (System.IO.IOException e)
                {
                    //Do nothing
                }
            }
            string destfile = path + Path.DirectorySeparatorChar + fileName + ext;
            try
            {
                File.Move(srcPath, destfile);
            }catch(System.IO.IOException e)
            {
                //pore desksi
            }
         }


        static void DirSearch(string sDir)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {
                        string ext = Path.GetExtension(f).ToLower();
                        if( ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                        {
                            jpgPathQuery.Enqueue(f);
                            count++;
                        }
                    }
                    DirSearch(d);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }
    }
}


