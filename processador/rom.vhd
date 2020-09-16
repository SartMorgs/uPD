-- -----------------------------------------------------
-- Arquivo ROM.vhd
-- -----------------------------------------------------
                                                        
library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
use IEEE.STD_LOGIC_UNSIGNED.ALL;
use IEEE.STD_LOGIC_ARITH.ALL;
   
   
   
entity ROM is 
  Generic ( 
      p_ADD_WIDTH    : INTEGER := 9; 
      p_DATA_WIDTH   : INTEGER := 16 
  ); 
  Port ( 
      i_CLK	   : in STD_LOGIC; 
      i_RST	   : in STD_LOGIC; 
      i_EN_CLK	   : in STD_LOGIC; 
      i_EN 	   : in STD_LOGIC; 
      i_ADDRESS      : in STD_LOGIC_VECTOR ((p_ADD_WIDTH-1) downto 0); 
      o_DOUT         : out STD_LOGIC_VECTOR ((p_DATA_WIDTH-1) downto 0) 
  ); 
end ROM; 
   
   
architecture Behavioral of ROM is 
      type ROM_TYPE is array(0 TO ((2**p_ADD_WIDTH)-1)) of std_logic_vector(o_DOUT'range); 
      signal ROM : ROM_TYPE := ((others=> (others=>'0'))); 
      signal w_ADDRESS	: STD_LOGIC_VECTOR(i_ADDRESS'range); 
   
  
begin 
   
   
-- -----------------------------------------------------
-- Codigo de maquina: 
-- -----------------------------------------------------
   
    ROM(0) <= "0010100000000000";    -- IN R0, 0;## PEGA ENTRADA TEXT
    ROM(1) <= "0100000000000000";    -- STO R0, 00;	## CARREGA ENTRADA
    ROM(2) <= "0000101000000001";    -- LDI R1, 1;		     ## 
    ROM(3) <= "0101110000100000";    -- AND R2, R0, R1;	     ##
    ROM(4) <= "0100110000000100";    -- JZ R2, 4;		     ##
    ROM(5) <= "0000101000010001";    -- LDI R1, 17 ;## (1---1)  carrega no R1
    ROM(6) <= "0101110000100000";    -- AND R2, R0, R1;		##  faz AND
    ROM(7) <= "1010011011000000";    -- CMP R3, R1, R2;		## Compara
    ROM(8) <= "0101001000001010";    -- JE R1, 10;	## Pula para linha 10
    ROM(9) <= "0000000000000000";    -- NOP;
    ROM(10) <= "0000111000000001";    -- LDI R3, 1;
    ROM(11) <= "0010011000000000";    -- OUT R3, 0;
    ROM(12) <= "0000000000000000";   
           
  w_ADDRESS <= (others=>'0') when(i_RST = '1') else i_ADDRESS; 
	U_ROM : process(i_CLK) 
	begin 
      if rising_edge (i_CLK) then 
          if (i_RST = '1') then 
              o_DOUT <= (others=>'0'); 
          else 
              if (i_EN = '1') then 
			o_DOUT <= ROM(conv_integer(w_ADDRESS)); 
              end if; 
          end if; 
      end if; 
	end process U_ROM; 
   
end Behavioral;            
