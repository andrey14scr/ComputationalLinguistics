import nltk
import json
from flask import Flask
from flask_restful import Resource, Api, reqparse
from nltk.stem import WordNetLemmatizer
from word_forms.word_forms import get_word_forms

app = Flask(__name__)
api = Api(app)

def parse(txt):
    parts = nltk.word_tokenize(txt)
    tokens = nltk.pos_tag(parts)
    arr = []
    offset = 0

    for token in tokens:
        word = token[0]
        if (word == '``' or word == "''"):
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

def getForms(word):
    all = get_word_forms(word)
    words = list()
    for set in all.values():
        for element in set:
            words.append(element)

    tokens = nltk.pos_tag(words)
    return json.dumps(tokens)

class Text(Resource):
    def post(self):
        parser = reqparse.RequestParser()
        parser.add_argument('text', required=True)
        args = parser.parse_args()
        answer = parse(args['text'])
        return {'answer': answer}, 200

class Word(Resource):
    def post(self):
        parser = reqparse.RequestParser()
        parser.add_argument('word', required=True)
        args = parser.parse_args()
        wl = WordNetLemmatizer()
        answer = wl.lemmatize(args['word'], pos="v")
        forms = getForms(args['word'])
        return {'init': answer, 'forms': forms}, 200

api.add_resource(Text, '/texts')
api.add_resource(Word, '/forms')

if __name__ == '__main__':
    nltk.download('wordnet')
    nltk.download('punkt')
    nltk.download('averaged_perceptron_tagger')
    app.run()