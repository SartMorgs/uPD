library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
use IEEE.STD_LOGIC_arith.ALL;
use IEEE.STD_LOGIC_unsigned.ALL;

entity MEMORIA is
	 Generic(
	 		p_DATA_WIDTH   : INTEGER := 16;        -- Número de bits dos dados. 
	 		p_ADD_WIDTH    : INTEGER := 6         -- Número de bits dos endereços. 
	 );
    Port ( 
		  i_CLK 	: in  STD_LOGIC;
		  i_RST   	: in  STD_LOGIC;
		  i_WR 		: in  STD_LOGIC;
		  i_ADDR  	: in  STD_LOGIC_VECTOR ((p_ADD_WIDTH-1) downto 0);
		  i_DATA  	: in  STD_LOGIC_VECTOR ((p_DATA_WIDTH-1) downto 0);
		  o_DATA  	: out STD_LOGIC_VECTOR ((p_DATA_WIDTH-1) downto 0)
	 );
end MEMORIA;

architecture Behavioral of MEMORIA is

--	component sram
--		PORT
--		(
--			aclr		: IN STD_LOGIC  := '0';
--			address	: IN STD_LOGIC_VECTOR (8 DOWNTO 0);
--			clock	: IN STD_LOGIC  := '1';
--			data		: IN STD_LOGIC_VECTOR (15 DOWNTO 0);
--			wren		: IN STD_LOGIC ;
--			q		: OUT STD_LOGIC_VECTOR (15 DOWNTO 0)
--		);
--	end component;

	type MEM_TYPE is array(0 TO ((2**p_ADD_WIDTH)-1)) of std_logic_vector(o_DATA'range);
	signal w_MEMORIA : MEM_TYPE;
	signal w_ADDR    : std_logic_vector(i_ADDR'range);
	
----------------------------------------------------------------------------------------	
begin

--	RAM : sram PORT MAP (
--			aclr	 	=> i_RST,
--			address	=> i_ADDR,
--			clock	=> i_CLK,
--			data	 	=> i_DATA,
--			wren	 	=> i_WR,
--			q	 	=> o_DATA
--	);


		w_ADDR <= (others=>'0') when (i_RST = '1') else i_ADDR;
	
		RAM : process(i_CLK)
		begin
			if rising_edge(i_CLK) then
				if (i_WR = '1') then
					w_MEMORIA(conv_integer(w_ADDR)) <= i_DATA;
				end if;
				o_DATA <= w_MEMORIA(conv_integer(i_ADDR));
			end if;
		end process RAM;
		
--		o_DATA <= w_MEMORIA(conv_integer(i_ADDR));
		
--		process(i_CLK)  begin
--			if falling_edge(i_CLK)  then
--		process(i_WR, i_ADDR, i_DATA)
--		begin
--				if (i_WR = '1') then
--					w_MEMORIA(conv_integer(i_ADDR)) <= i_DATA;
--				end if;
--							
--				o_DATA <= w_MEMORIA(conv_integer(i_ADDR));
--			end if;	
--		end process;

		
		
end Behavioral;
