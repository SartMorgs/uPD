library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
use IEEE.STD_LOGIC_ARITH.ALL;
use IEEE.STD_LOGIC_UNSIGNED.ALL;


entity REGISTRADOR is
	 Generic (
				p_DATA_WIDTH		: INTEGER := 12
	 );
    Port ( 
				i_CLK   	: in  STD_LOGIC;
				i_RST   	: in  STD_LOGIC;
				i_DATA	: in  STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);	  
				o_DATA	: out STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);	
			     i_WR   	: in  STD_LOGIC	
	 );
end REGISTRADOR;

architecture Behavioral of REGISTRADOR is

	
begin

--	process(i_CLK)
--	begin
--		if rising_edge (i_CLK) then
--			if (i_RST = '1') then
--				o_DATA <= (others => '0');
--			else	
--				if (i_WR = '1') then
--					o_DATA <= i_DATA;
--				end if;
--			end if;
--		end if;
--	end process;
	
	process(i_CLK, i_RST)
	begin
		if (i_RST = '1') then
			o_DATA <= (others => '0');
		elsif rising_edge (i_CLK) then
			if (i_WR = '1') then
				o_DATA <= i_DATA;
			end if;
		end if;
	end process;	

end behavioral;
	