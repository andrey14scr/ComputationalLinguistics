import nltk
import json
from flask import Flask
from flask_restful import Resource, Api, reqparse

app = Flask(__name__)
api = Api(app)

def parse(txt):
    parts = nltk.word_tokenize(txt)
    mylist = nltk.pos_tag(parts)
    arr = []
    offset = 0

    for token in mylist:
        x = {
            "word": token[0],
            "prop": token[1],
            "offset": offset
        }
        offset = txt.find(token[0], offset)
        arr.append(x)
        offset += len(token[0])

    return json.dumps(arr)

class Text(Resource):
    def post(self):
        parser = reqparse.RequestParser()
        parser.add_argument('text', required=True)
        args = parser.parse_args()
        answer = parse(args['text'])
        return {'answer': answer}, 200

api.add_resource(Text, '/texts')

if __name__ == '__main__':
    app.run()