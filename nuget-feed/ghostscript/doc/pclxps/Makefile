DOCS=ghostpdl.pdf ghostpdl.txt

# latex 2 pdf
%.pdf : %.tex
	pdflatex $<

# pdf 2 text
%.txt : %.pdf
	pdftotext $<

all: $(DOCS)

clean:
	rm -f $(DOCS)
