import { Directive, ElementRef, Input, OnChanges, SimpleChanges } from '@angular/core';

@Directive({
  selector: '[appTypingAnimation]'
})
export class TypingAnimationDirective implements OnChanges {
  @Input() appTypingAnimation: string = '';

  constructor(private el: ElementRef) {}

  ngOnChanges(changes: SimpleChanges) {
    if (changes['appTypingAnimation']) {
      this.displayWithTypingEffect(changes['appTypingAnimation'].currentValue);
    }
  }

  private displayWithTypingEffect(text: string) {
    this.el.nativeElement.textContent = '';
    let i = 0;
    const speed = 20; // milliseconds between characters

    const typingEffect = () => {
      if (i < text.length) {
        this.el.nativeElement.textContent += text.charAt(i);
        i++;
        setTimeout(typingEffect, speed);
      }
    };

    typingEffect();
  }
}