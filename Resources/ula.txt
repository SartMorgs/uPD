library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
use IEEE.STD_LOGIC_ARITH.ALL;
use IEEE.STD_LOGIC_UNSIGNED.ALL;


entity ULA is
    Generic(
			p_DATA_WIDTH    : INTEGER := 16
	 );	
    Port ( 
			i_REG_S1		: in  STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);	  
			i_REG_S2		: in  STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);	
			i_SEL_ULA   	: in  STD_LOGIC_VECTOR(2 DOWNTO 0);	
			o_DATA_ULA	: out  STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0)
	 );
end ULA;

architecture Behavioral of ULA is

	
begin

	process (i_SEL_ULA, i_REG_S1, i_REG_S2 )
	begin
			if (i_SEL_ULA = "000") then
				o_DATA_ULA <= i_REG_S1 + i_REG_S2;
				
			elsif (i_SEL_ULA = "001") then
				o_DATA_ULA <= i_REG_S1 - i_REG_S2;
				
			elsif (i_SEL_ULA = "010") then
				o_DATA_ULA <= i_REG_S1 and i_REG_S2;
	
			elsif (i_SEL_ULA = "011") then
				o_DATA_ULA <= i_REG_S1 or i_REG_S2;

			elsif (i_SEL_ULA = "100") then
				o_DATA_ULA <= i_REG_S1 xor i_REG_S2;

			elsif (i_SEL_ULA = "101") then
				o_DATA_ULA <= not i_REG_S1;

			elsif (i_SEL_ULA = "110") then
				if (i_REG_S1 = i_REG_S2) then
					o_DATA_ULA((p_DATA_WIDTH-1) downto 1) <= (others => '0');
					o_DATA_ULA(0) <= '1';
				else
					o_DATA_ULA <= (others => '0');
				end if;
				
			else 
				if (i_REG_S1 = x"0000") then
					o_DATA_ULA((p_DATA_WIDTH-1) downto 1) <= (others => '0');
					o_DATA_ULA(0) <= '1';
				else
					o_DATA_ULA <= (others => '0');
				end if;			
			end if;
	end process;
	

end behavioral;
	