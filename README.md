<!-- TABLE OF CONTENTS -->
## Table of Contents

* [Sobre o Projeto](#sobre-o-projeto)
  * [Intuito](#intuito)
  * [Equipe](#time)
* [Sobre o Processador](#sobre-o-processador)
  * [Formato de Instrução](#formato-de-instrução)
  * [Como foi Desenvolvido](#como-foi-desenvolvido)
* [Como Usar](#como-usar)
* [Como Contribuir](#como-contribuir)
* [Roadmap de Melhorias](#roadmap-de-melhorias)
   
<!-- ABOUT THE PROJECT -->
## Sobre o Projeto
O μPD surgiu como uma ferramenta para ajudar no ensino de Arquitetura de Computadores, sendo um processador monociclo cujo pode ter todas as suas instruções simuladas visualmente através de um software de simulação. O projeto foi expandido para uma IDE onde era possível gerar um programa no assemvbly do μPD, compilar e executar, além de gerar o binário do programa desenvolvido. Também foi acrescentado uma ferramenta de geração do "esqueleto" do seu código VHDL (o qual ele foi desenvolvido) para uso em um kit educacional de FPGA.
### Intuito
O objetivo sempre foi contribuir com o aprendizado dos alunos, englobando em um mesmo processador a possibilidade de aprendizado de várias áreas da computação em conjunto.
### Equipe
Quem projetou a arquitetura do processador, bem como boa parte do código em VHDL foi o [Marcelo Daniel Berejuck](https://linkedin.com/in/marcelo-daniel-berejuck-0b923064). A [Morgana Sartor](https://linkedin.com/in/morgana-sartor) foi quem ajudou no desenvolvimento do código VHDL e testes, e foi uma das programadoras do software de simulação. A [Thaynara Mitie](https://linkedin.com/in/thaynara-mitie) foi uma das programadoras do software e responsável por seus testes de funcionamento.

<!-- ABOUT THE PROCESSOR -->
## Sobre o Processador
O μPD utiliza uma arquitetura baseada na arquitetura RISC (Reduced Instruction-Set Computer), o que significa que todas as operações são realizadas com o uso dos registradores. A arquitetura RISC é muitas vezes chamada de “arquitetura Load/Store”. O processador μPD possui um Banco de Registradores com quatro registradores de uso geral (R0, R1, R2 e R3) que podem ser utilizados pelo usuário sem nenhuma restrição.
### Formato de Instrução
As instruções do processador possuem dois seis tipos de formatos, conforme mostra a Figura abaixo. Operações que envolvem três registradores utilizam o formato mostrado na Figura (a), operações que envolvem apenas um registrador destino e outro de entrada estão representadas na Figura (b), operações apenas com os dois registradores de entrada estão na Figura (c), e as operações apenas com registrador de destino estão representadas na Figura (d). Também existem operações que não envolvem registradores, elas estão representadas nas figuras (e) e (f). Abaixo estão descritas as instruções.
| Mnemônico | Opcode (binário) | Opcode (hexadecimal) | Descrição |
|-----------|------------------|----------------------|-----------|
| NOP       | 00000            | 00                   | “No operation”. Processador gasta um ciclo de relógio se fazernenhuma operação.|
| LDI       | 00001            | 01                   | Carrega valor imediato contido na instrução (bits de 0 a 8) em umdeterminado Registrador (RDST).|
| ADD       | 00010            | 02                   | Soma o conteúdo de dois registradores e armazena o resultado emum terceiro registrador.|
| SUB       | 00011            | 03                   | Subtrai o conteúdo de dois registradores e armazena o resultado emum terceiro registrador.|
| OUT       | 00100            | 04                   | Escreve o valor contido em um registrador (R0, R1, R2 ou R3) nos pinos de I/O.|
### Como foi Desenvolvido


<!-- HOW TO USE -->
## Como Usar


<!-- HOW TO CONTRIBUTE -->
## Como Contribuir


<!-- ROADMAP -->
## Roadmap de Melhorias
