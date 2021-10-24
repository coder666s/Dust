using System;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using OpenPop.Common;
using OpenPop.Pop3;
using kNNdll;
using System.Threading;
using ImageClassification;
using ImageClassification.IO;

namespace PetsFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            //Thread t1 = new Thread(new ThreadStart(MailChecking));//запускает чекер для почты
            //t1.Start(); // запускаем поток
            Thread n = new Thread(new ParameterizedThreadStart(NeuronChecker));//чисто тест одной картинки
            n.Start("downloads/test.jpg");
        }


        static List<string> UIDS = new List<string>();//список уникальных идентификаторов писем. если идентификатор письма есть в списке - значит оно уже читалось, и теперь проигнорируется.
        static void MailChecking()
        {
            var dir = Directory.GetCurrentDirectory() + "\\downloads\\";//локальная папка для сохранения картинок (временная)
            if (File.Exists("uids")) UIDS = new List<string>(File.ReadAllLines("uids"));
            var client = new Pop3Client();//создаем клиента
            client.Connect("pop.gmail.com", 995, true);//подключаемся и входим
            client.Authenticate("petfinder1385@gmail.com", "ASD456asd", AuthenticationMethod.UsernameAndPassword);
            while (true)//бесконечная проерка
            {
                bool B = false;
                var count = client.GetMessageCount();//получаем кол-во писем
                if (count > 0)
                {
                    for (int i = 1; i <= count; i++)
                    {
                        var m = client.GetMessage(i);//получаем само письмо
                        var uid = client.GetMessageUid(i);//его ИД
                        Console.WriteLine(i + ": " + uid + "; " + m.Headers.Date);
                        if (!UIDS.Contains(uid))//если ида нет в списке, то это новое письмо
                        {
                            var ass = m.FindAllAttachments();//все прикрепленные файлы
                            for (int j = 0; j < ass.Count; j++)
                            {
                                Console.WriteLine(dir + ass[j].FileName);
                                var tp = ass[j].ContentType.MediaType;//берем тип файла
                                Console.WriteLine(tp);
                                if (tp.EndsWith("png") | tp.EndsWith("jpg") | tp.EndsWith("jpeg"))//если это картинка
                                {
                                    ass[j].Save(new FileInfo(dir + ass[j].FileName));//то сохраняем
                                    Thread n = new Thread(new ParameterizedThreadStart(NeuronChecker));
                                    n.Start(dir + ass[j].FileName);
                                }
                            }
                            UIDS.Add(uid);
                            //client.DeleteMessage(i);//планируется очищать прочитанные сообщения. ещё не протестировано
                            B = true;
                        }
                        Console.WriteLine("\n");
                    }
                    //client.DeleteAllMessages();
                    if (B) File.WriteAllLines("uids", UIDS.ToArray());
                }
                client.Disconnect();//из-за специфического принципа работы клиент не обновляет список писем в режиме реального времени, поэтому приходится переподключаться
                client.Connect("pop.gmail.com", 995, true);
                client.Authenticate("petfinder1385@gmail.com", "ASD456asd", AuthenticationMethod.UsernameAndPassword);
            }
        }
    
        static string Cameras = Directory.GetCurrentDirectory() + "\\cameras\\";
        static MNN.answ[] Answer;
        static MNN.answ[][] Matrix;
        static bool b = false;
        static void NeuronChecker(object PhotoName)
        {
            var p = Classifier.GetSingleImagePrediction("pipeline.zip", "model.zip", (string)PhotoName);
            Console.WriteLine($@"Predicted image label is: ""{p.PredictedLabel}"". Score:{p.HighScore}");
            Console.Write(p.LabelsScore.Count);
            MNN.Q = p.LabelsScore.Count;
            MNN.Point200 p200 = new MNN.Point200();
            for (int i = 0; i < p.LabelsScore.Count; i++) p200.axis[i] = p.LabelsScore[i].fScore;
            MNN.Point200[] points = new MNN.Point200[p.LabelsScore.Count];
            for (int i = 0; i < p.LabelsScore.Count; i++)
            {
                points[i] = new MNN.Point200();
                points[i].axis[i] = 1;
                points[i].type = p.LabelsScore[i].Label;
            }
            Answer = MNN.MP(MNN.New(p.LabelsScore.Count, 2, p200, points));
            
            for (int i = 0; i < Answer.Length; i++)
            {
                Console.WriteLine(Answer[i].type);
                Console.WriteLine(Answer[i].proc);
                Console.WriteLine("\n");
            }

            //
            string[] Screens = Directory.GetFiles(Cameras, "*", SearchOption.AllDirectories);
            //

            if (!b)
            {
                Matrix = new MNN.answ[Screens.Length][];
                b = true;
            }

            for (int i = 0; i < Screens.Length; i++)
            {
                int n = i;
                CameraChecker(n, Screens[n]);
                Console.WriteLine((i + 1) + " / " + (Screens.Length));
            }
            Console.WriteLine("Сравнение:");
            for (int i = 0; i < Matrix.Length; i++)
            {
                Console.WriteLine(i + " / " + Matrix.Length + ":");
                int c = 0;
                for (int j = 0; j < Matrix[i].Length; j++)
                {
                    if (Math.Abs(Answer[j].proc - Matrix[i][j].proc) < 1)
                    {
                        c++;
                        Console.WriteLine(j + ". " + Math.Abs(Answer[j].proc - Matrix[i][j].proc));
                    }
                }
                if (c == Matrix[i].Length)
                {
                    Console.WriteLine("Собака нашлась на камере " + Screens[i] + " (" + c + ")");
                    Console.WriteLine("\n");
                }
            }
        }

        static void CameraChecker(int N, string S)
        {
            var p = Classifier.GetSingleImagePrediction("pipeline.zip", "model.zip", S);
            MNN.Point200 p200 = new MNN.Point200();
            for (int i = 0; i < p.LabelsScore.Count; i++) p200.axis[i] = p.LabelsScore[i].fScore;
            MNN.Point200[] points = new MNN.Point200[p.LabelsScore.Count];
            for (int i = 0; i < p.LabelsScore.Count; i++)
            {
                points[i] = new MNN.Point200();
                points[i].axis[i] = 1;
                points[i].type = p.LabelsScore[i].Label;
            }
            Matrix[N] = MNN.MP(MNN.New(p.LabelsScore.Count, 2, p200, points));
            //L++;
        }
    
    }
}
