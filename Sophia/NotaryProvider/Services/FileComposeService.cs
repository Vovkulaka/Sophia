using DataProvider.Infrastructure.Extantions;
using MessageModelLib;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NotaryProvider.Services
{
    public interface IFileComposeService
    {
        Task AddOrUpdate(MessageFileModel model);
    }

    public class FileComposeService : IFileComposeService
    {
        private Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// https://habr.com/ru/company/skbkontur/blog/348508/
        /// </summary>
        private static ConcurrentDictionary<string, ConcurrentBag<MessageFileModel>> CollectoinFile { get; } = new ConcurrentDictionary<string, ConcurrentBag<MessageFileModel>>();
        private ConcurrentBag<MessageFileModel> collectionMessage { get; set; } = null; 

        public Task AddOrUpdate(MessageFileModel model)
        {
            Logger.AddLogStart($"METHOD: AddOrUpdate");

            try
            {
                // Добавляешь model в коллекцию ConcurrentBag, если IdModel есть.
                // Создаёшь новую пару ключ-значение, если такого файла ещё нет.
                var cbExiste = CollectoinFile.FirstOrDefault(kv => kv.Key.Contains(model.IdFile));

                // БУДЬ ЯКЕ ЛОГУВАННЯ В try НЕ ПРАЦЮЄ!!!
                // ВИКОИРСТОВУЙ МЕТОД:
                //MyLog2(model, cbExiste); // хоча тут навіть так не працює!!!

                if (CollectoinFile.IsEmpty || cbExiste.Key != model.IdFile) // ЯКЩО cd ПУСТА || ЯКЩО cd НЕ ПУСТА АЛЕ В cd НЕ ІСНУЄ cb ФАЙЛУ ПОВІДОМЛЕННЯ ЯКОГО ЗАРАЗ ОБРОБЛЯЄТЬСЯ 
                {
                    AddFirstCB(model);
                    //if (collectionMessage == null)
                    //{
                    //    collectionMessage = new ConcurrentBag<MessageFileModel>();
                    //}
                    //collectionMessage.Add(model);
                    if (cbExiste.Key != null)
                    {
                        MyLog(model, cbExiste);
                    }
                }
                else if (cbExiste.Key == model.IdFile) // ЯКЩО В cd ІСНУЄ cb ФАЙЛУ ПОВІДОМЛЕННЯ ЯКОГО ЗАРАЗ ОБРОБЛЯЄТЬСЯ
                {
                    ConcurrentBag<MessageFileModel> collectionMessage = cbExiste.Value;
                    AddFirstCB(model, collectionMessage);
                    //if (collectionMessage == null)
                    //{
                    //    collectionMessage = new ConcurrentBag<MessageFileModel>();
                    //}
                    //collectionMessage.Add(model);

                    MyLog(model, cbExiste);
                }
            }
            catch (Exception ex)
            {
                Logger.AddLogException(ex);
            }
            return Task.CompletedTask;
        }
        private void AddFirstCB (MessageFileModel model, ConcurrentBag<MessageFileModel> collectionMessage = null)
        {
            #region Add to ConcurrentBag concurrently
            if (collectionMessage == null)
            {
                collectionMessage = new ConcurrentBag<MessageFileModel>();
            }
            List<Task> bagAddTasks = new List<Task>();
            bagAddTasks.Add(Task.Run(() => collectionMessage.Add(model)));

            // Wait for all tasks to complete
            Task.WaitAll(bagAddTasks.ToArray()); 
            #endregion

            CollectoinFile.TryAdd(model.IdFile, collectionMessage);
            Thread.Sleep(1000);
        }

        private void MyLog(MessageFileModel model, KeyValuePair<string, ConcurrentBag<MessageFileModel>> cbExiste)
        {
            Logger.AddLogStart($"IN ConcurrentDictionary<string, ConcurrentBag<MessageModel>> THE NUMBER ConcurrentBag<MessageModel> IS: " + CollectoinFile.Count);
            Logger.AddLogStart($"IN ConcurrentBag<MessageModel>> THE NUMBER MessageModel FROM IdFile: " + model.IdFile + " IS: " + cbExiste.Value.Count);
        }

        public IEnumerable<IEnumerable<MessageFileModel>> Read()
        {
            // Итерируешь коллекцию файлов, чтобы можно было это обрабатывать в hosted service.
            foreach (ConcurrentBag<MessageFileModel> part in CollectoinFile.Values)
            {
                if (part.FirstOrDefault().Quantity == part.Count - 1)
                {
                    yield return part.AsEnumerable();
                }
            }
        }

        public void DeleteCBModel (string IdFile)
        {
            try
            {
                ConcurrentBag<MessageFileModel> models;
                bool deleted = CollectoinFile.TryRemove(IdFile, out models);
                if (deleted)
                {
                    Logger.AddLogStart("ConcurrentDictionary<string, ConcurrentBag<MessageModel>> - CLEANED");
                }
            }
            catch (Exception ex)
            {
                Logger.AddLogException(ex);
            }
        }
    }
}
