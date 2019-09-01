using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace Sort_Image_by_Date_Taked
{
    class Program
    {
        static public Queue<string> jpgPathQuery = new Queue<string>(); //Queue to hold image paths
        static public int count;                                        //Count total jpg,jpeg, png
        static public string DestinationPath;                           //Destinationp path for image
        static public string dirName, fileName;                                //global variable needed to store directory name and file na,e

        static int Main(string[] args)
        {

            //Making the destination path
            DestinationPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + Path.DirectorySeparatorChar + "export";

            //First arg should be src directory path
            if (args.Length > 0)
            {
                string path = args[0];
                if (String.Empty != path)
                {
                    try
                    {
                        FileAttributes attr = File.GetAttributes(path);             //Make sure it is directory
                        if (attr.HasFlag(FileAttributes.Directory)) {

                            DirSearch(path);                                        //Start Recursive Search //TODO: Make it multi threaded

                            Console.WriteLine("Total enqueued: " + count.ToString());   //Outout Total image count

                            //Start Denquing and Moding
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
            string image = jpgPathQuery.Dequeue();                  //Pull fisrt image
            string ext = Path.GetExtension(image).ToLower();        //Get the extention

            //using bitmap to read propertyItems
            using (Bitmap b = new Bitmap(image))
            {
                PropertyItem[] propItems = b.PropertyItems;

                ASCIIEncoding encodings = new ASCIIEncoding();      //Enconding to human readable code

                foreach (PropertyItem item in propItems)
                {
                    if(item.Id == 0x9003)                           //Property ID 0x9003 holds datetime of image taken value
                    {
                        try
                        {
                            string DT = encodings.GetString(item.Value);
                            Console.WriteLine("The Date & Time is " + DT.Replace(":","-"));


                                string[] tmp = DT.Replace(":", "-").Split(' ');
                                dirName = tmp[0];
                                fileName = tmp[1].Substring(0,tmp[1].Length - 1);


                        }
                        catch(EncoderFallbackException e)
                        {
                            Console.WriteLine("No Date Time");
                        }
                        Console.WriteLine("");
                    }
                   
                }
            }
            //This move command is called here beause inside using block the file is locked with a handler
            Move(image, DestinationPath, dirName, fileName, ext);
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


