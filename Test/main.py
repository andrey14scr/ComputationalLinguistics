import nltk
import json

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
    t = 'I am in "Minsk" city now.'
    result = task(t)
    print(result)