import nltk
import json

def task(txt):
    parts = nltk.word_tokenize(txt)
    tokens = nltk.pos_tag(parts)
    mylist = list(dict.fromkeys(tokens))
    arr = []
    offset = 0

    for token in mylist:
        if token[0].isalpha():
            x = {
                "word": token[0].lower(),
                "prop": token[1],
                "offset": offset
            }
            offset = txt.find(token[0], offset)
            arr.append(x)
            offset += len(token[0])

    return json.dumps(arr)

if __name__ == '__main__':
    print("lol")