library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
use IEEE.STD_LOGIC_ARITH.ALL;
use IEEE.STD_LOGIC_UNSIGNED.ALL;


entity INTERRUPCAO is
    Port ( 
			i_CLK   	: in  STD_LOGIC;
			i_RST   	: in  STD_LOGIC;
			i_INT_ADD	: in  STD_LOGIC_VECTOR(1 DOWNTO 0);
			i_INT0		: in  STD_LOGIC;
			i_INT1		: in  STD_LOGIC;
			i_INT2		: in  STD_LOGIC;
			o_BUSY      : out STD_LOGIC;
			o_INT0		: out STD_LOGIC;
			o_INT1		: out STD_LOGIC;
			o_INT2		: out STD_LOGIC			
	 );
end INTERRUPCAO;

architecture Behavioral of INTERRUPCAO is
	signal w_INT0		: STD_LOGIC;
	signal w_INT1		: STD_LOGIC;
	signal w_INT2		: STD_LOGIC;
	
	
begin

	w_INT0 <= i_INT0;
	w_INT1 <= i_INT1;
	w_INT2 <= i_INT2;
	
	o_INT0 <= w_INT0;
	o_INT1 <= w_INT1 and (not w_INT0);
	o_INT2 <= w_INT2 and (not w_INT0) and (not w_INT1);
	
	
	process(i_CLK, i_RST)
	begin
		if (i_RST = '1') then
			o_BUSY <= '0';
		elsif rising_edge (i_CLK) then
			if (i_INT_ADD = "00") then
				o_BUSY <= '0';
			else
				o_BUSY <= '1';
			end if;
		end if;
	end process;	

end behavioral;
	