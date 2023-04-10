using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

public class ConversationLoader : MonoBehaviour
{
    public string jsonFileName;
    private Dictionary<string, List<Message>> conversationDict;

    void Start()
    {
        LoadConversations(jsonFileName);
    }

    private void LoadConversations(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);
            ParseConversations(jsonString);
        }
        else
        {
            Debug.LogError("JSON file not found: " + filePath);
        }
    }

    private void ParseConversations(string jsonString)
    {
        ConversationData conversationData = JsonConvert.DeserializeObject<ConversationData>(jsonString);
        conversationDict = new Dictionary<string, List<Message>>();

        foreach (Thread thread in conversationData.threads)
        {
            string contactId = thread.contact.id;
            if (!conversationDict.ContainsKey(contactId))
            {
                conversationDict[contactId] = new List<Message>();
            }

            conversationDict[contactId].AddRange(thread.msgs);
        }

        // Debug log each message list  
        foreach (KeyValuePair<string, List<Message>> entry in conversationDict)
        {
            Debug.Log("Key: " + entry.Key);
            foreach (Message message in entry.Value)
            {
                Debug.Log("Message: " + message.txt);
            }
        }
    }

    [System.Serializable]
    public class ConversationData
    {
        public List<Thread> threads;
    }

    [System.Serializable]
    public class Thread
    {
        public int id;
        public string app;
        public Contact contact;
        public List<Message> msgs;
    }

    [System.Serializable]
    public class Contact
    {
        public string name;
        public string id;
    }

    [System.Serializable]
    public class Message
    {
        public string time;
        public string uid;
        public string txt;
    }
}
