using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JsonParser : MonoBehaviour
{
    [System.Serializable]
    public class Message
    {
        public string senderId;
        public string content;
        public string timestamp;
    }

    [System.Serializable]
    public class Contact
    {
        public string id;
        public string name;
    }

    [System.Serializable]
    public class Conversation
    {
        public string id;
        public string appId;
        public Contact contact;
        public Message[] messages;
    }

    public string jsonFileName = "conversations.json";
    public bool useRelativeTime = true;
    private Dictionary<string, List<Message>> conversationThreads;

    void Start()
    {
    conversationThreads = new Dictionary<string, List<Message>>();

    string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);

    if (File.Exists(filePath))
    {
        string dataAsJson = File.ReadAllText(filePath);

        Conversation[][] conversations = JsonHelper.FromJson<Conversation[][]>(dataAsJson);

        foreach (var thread in conversations)
        {
            string threadId = thread[0].id;

            List<Message> messages = new List<Message>();

            foreach (var conversation in thread)
            {
                messages.AddRange(conversation.messages);
            }

            if (useRelativeTime)
            {
                ConvertTimestampToRelativeTime(messages);
            }

            conversationThreads.Add(threadId, messages);

            foreach (var message in messages)
            {
                Debug.Log("Thread ID: " + threadId);
                Debug.Log("Sender ID: " + message.senderId);
                Debug.Log("Content: " + message.content);
                Debug.Log("Timestamp: " + message.timestamp);
            }
        }
    }
    else
    {
        Debug.LogError("Cannot find file!");
    }
}

    private void ConvertTimestampToRelativeTime(List<Message> messages)
    {
        DateTime currentDate = DateTime.Now;

        foreach (var message in messages)
        {
            DateTime timestamp = DateTime.Parse(message.timestamp);

            if (timestamp.Date == currentDate.Date)
            {
                TimeSpan timeDiff = currentDate - timestamp;

                if (timeDiff.TotalMinutes < 1)
                {
                    message.timestamp = "just now";
                }
                else if (timeDiff.TotalMinutes < 60)
                {
                    message.timestamp = string.Format("{0} minutes ago", (int)timeDiff.TotalMinutes);
                }
                else
                {
                    message.timestamp = string.Format("today, {0}", timestamp.ToString("h:mm tt").ToLower());
                }
            }
            else if (timestamp.Date == currentDate.AddDays(-1).Date)
            {
                message.timestamp = string.Format("yesterday, {0}", timestamp.ToString("h:mm tt").ToLower());
            }
            else
            {
                message.timestamp = timestamp.ToString("M/d/yy");
            }
        }
    }
}

public static class JsonHelper
{
    public static T FromJson<T>(string json)
    {
        return JsonUtility.FromJson<T>(json);
    }

    public static string ToJson<T>(T obj)
    {
        return JsonUtility.ToJson(obj);
    }

    public static string ToJson<T>(T obj, bool prettyPrint)
    {
        return JsonUtility.ToJson(obj, prettyPrint);
    }

    public static T[] FromJsonArray<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJsonArray<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJsonArray<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
