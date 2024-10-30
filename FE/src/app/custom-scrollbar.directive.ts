import { Directive, ElementRef, Renderer2 } from '@angular/core';

@Directive({
  selector: '[appCustomScrollbar]'
})
export class CustomScrollbarDirective {
  constructor(private el: ElementRef, private renderer: Renderer2) {
    this.setStyles();
  }

  private setStyles() {
    const styles = `
      ::-webkit-scrollbar {
        width: 12px;
      }
      ::-webkit-scrollbar-thumb {
        background-color: #28a745;
        border-radius: 10px;
      }
      ::-webkit-scrollbar-track {
        background: #e9f7ef;
        border-radius: 10px;
      }
      ::-moz-scrollbar {
        width: 12px;
      }
      ::-moz-scrollbar-thumb {
        background-color: #28a745;
        border-radius: 10px;
      }
      ::-moz-scrollbar-track {
        background: #e9f7ef;
        border-radius: 10px;
      }
      ::-ms-scrollbar {
        width: 12px;
      }
      ::-ms-scrollbar-thumb {
        background-color: #28a745;
        border-radius: 10px;
      }
      ::-ms-scrollbar-track {
        background: #e9f7ef;
        border-radius: 10px;
      }
      ::-o-scrollbar {
        width: 12px;
      }
      ::-o-scrollbar-thumb {
        background-color: #28a745;
        border-radius: 10px;
      }
      ::-o-scrollbar-track {
        background: #e9f7ef;
        border-radius: 10px;
      }
      ::scrollbar {
        width: 12px;
      }
      ::scrollbar-thumb {
        background-color: #28a745;
        border-radius: 10px;
      }
      ::scrollbar-track {
        background: #e9f7ef;
        border-radius: 10px;
      }
    `;
    const styleElement = this.renderer.createElement('style');
    this.renderer.appendChild(styleElement, this.renderer.createText(styles));
    this.renderer.appendChild(this.el.nativeElement, styleElement);
  }
}
