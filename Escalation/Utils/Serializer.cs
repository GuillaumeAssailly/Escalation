using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Escalation.Utils
{
    internal abstract class EarthSaver
    {
        protected List<Memento> history = new List<Memento>();


        public void SaveState(Memento memento)
        {
            history.Add(memento);
        }

        public Memento GetState(int index)
        {
            return history[index];
        }

        public void RestoreState(int index)
        {
            history[index] = history.Last();
            history.RemoveAt(history.Count - 1);
        }

        public void DeleteState(int index)
        {
            history.RemoveAt(index);
        }

        public void DeleteAll()
        {
            history.Clear();
        }

        public void DeleteLast()
        {
            history.RemoveAt(history.Count - 1);
        }

        protected EarthSaver() { }

        public abstract void SaveLast(string path);

        public abstract void SaveAll(string path);

        public abstract void SaveLastAndRemove(string path);

    }


    internal class JsonEarthSaver : EarthSaver
    {
        public JsonEarthSaver() : base()
        {

        }


        public override void SaveLast(string path)
        {
            FileWriter.AppendLine(path, JsonSerializer.Serialize(history.Last().GetState(), new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            }));
        }

        public override void SaveAll(string path)
        {
            FileWriter.AppendLine(path, JsonSerializer.Serialize(history));
        }

        public override void SaveLastAndRemove(string path)
        {
            FileWriter.AppendLine(path, JsonSerializer.Serialize(history.Last().GetState(), new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            }));
            DeleteLast();
        }
    }
}
