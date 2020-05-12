using DataProvider.Infrastructure.Extantions;
using MessageModelLib;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sophia.FrontApi.Services
{
    public interface IResponseComposeService
    {
        Task AddOrUpdate(MessageResponseModel model);
    }
    public class ResponseComposeService : IResponseComposeService
    {
        private Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// https://habr.com/ru/company/skbkontur/blog/348508/
        /// </summary>
        // Коли додасиш ще один продюсер, то для розділення 
        private static ConcurrentDictionary<string, ConcurrentBag<MessageResponseModel>> colectoin { get; } = 
            new ConcurrentDictionary<string, ConcurrentBag<MessageResponseModel>>();

        public Task AddOrUpdate(MessageResponseModel model)
        {
            Logger.AddLogStart($"METHOD: AddOrUpdate");

            try
            {
                // Потім перероби цей метод так щоб  ConcurrentBag, если IdModel есть.
                // Создаёшь новую пару ключ-значение, если такого файла ещё нет.
                var cbExiste = colectoin.FirstOrDefault(kv => kv.Key.Contains(model.IdRequest));

                // БУДЬ ЯКЕ ЛОГУВАННЯ В try НЕ ПРАЦЮЄ!!!
                // ВИКОИРСТОВУЙ МЕТОД: MyLog
                //MyLog2(model, cbExiste); // хоча тут навіть так не працює!!!

                if (colectoin.IsEmpty || cbExiste.Key != model.IdRequest) // ЯКЩО colectoin ПУСТА || ЯКЩО colectoin НЕ ПУСТА АЛЕ В colectoin НЕ ІСНУЄ cb ФАЙЛУ ПОВІДОМЛЕННЯ ЯКОГО ЗАРАЗ ОБРОБЛЯЄТЬСЯ 
                {
                    AddFirstCB(model);
                    if (cbExiste.Key != null)
                    {
                        MyLog(model, cbExiste);
                    }
                }
                else if (cbExiste.Key == model.IdRequest) // ЯКЩО В colectoin ІСНУЄ cb ФАЙЛУ ПОВІДОМЛЕННЯ ЯКОГО ЗАРАЗ ОБРОБЛЯЄТЬСЯ
                {
                    ConcurrentBag<MessageResponseModel> cb = cbExiste.Value;
                    AddFirstCB(model, cb);

                    MyLog(model, cbExiste);
                }
            }
            catch (Exception ex)
            {
                Logger.AddLogException(ex);
            }
            return Task.CompletedTask;
        }
        private void AddFirstCB(MessageResponseModel model, ConcurrentBag<MessageResponseModel> cb = null)
        {
            #region Add to ConcurrentBag concurrently
            if (cb == null)
            {
                cb = new ConcurrentBag<MessageResponseModel>();
            }
            List<Task> bagAddTasks = new List<Task>();
            bagAddTasks.Add(Task.Run(() => cb.Add(model)));

            // Wait for all tasks to complete
            Task.WaitAll(bagAddTasks.ToArray());
            #endregion

            colectoin.TryAdd(model.IdRequest, cb);
        }

        private void MyLog(MessageResponseModel model, KeyValuePair<string, ConcurrentBag<MessageResponseModel>> cbExiste)
        {
            Logger.AddLogStart($"IN ConcurrentDictionary<string, ConcurrentBag<MessageModel>> THE NUMBER ConcurrentBag<MessageModel> IS: " + colectoin.Count);
            Logger.AddLogStart($"IN ConcurrentBag<MessageModel>> THE NUMBER MessageModel FROM IdRequest: " + model.IdRequest + " IS: " + cbExiste.Value.Count);
        }

        public IEnumerable<IEnumerable<MessageResponseModel>> Read()
        {
            foreach (ConcurrentBag<MessageResponseModel> part in colectoin.Values)
            {
                yield return part.AsEnumerable();
            }
        }
    }
}
