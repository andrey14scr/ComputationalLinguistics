import nltk
import json
from nltk.stem import WordNetLemmatizer
from word_forms.word_forms import get_word_forms

def task(txt):
    parts = nltk.word_tokenize(txt)
    tokens = nltk.pos_tag(parts)
    arr = []
    offset = 0

    for token in tokens:
        word = token[0]
        if(word == '``' or word == "''"):
            word = '"'

        x = {
            "word": word.lower(),
            "prop": token[1],
            "offset": offset
        }
        offset = txt.find(word, offset)
        arr.append(x)
        offset += len(word)

    return json.dumps(arr)

if __name__ == '__main__':
    nltk.download('wordnet')
    nltk.download('punkt')
    nltk.download('averaged_perceptron_tagger')
    #t = 'I am in "Minsk" city now.'
    #result = task(t)
    #print(result)wordnet

    wl = WordNetLemmatizer()
    #print(wl.lemmatize("downloading", pos="v"))

    all = get_word_forms("was")
    words = list()
    for e in all.values():
        for f in e:
            words.append(f)

    tokens = nltk.pos_tag(words)
    print(json.dumps(tokens))

