import nltk

text = nltk.word_tokenize("A Python is a serpent which eats eggs from the nest")
tagged_text=nltk.pos_tag(text)
print(tagged_text)